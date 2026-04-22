using FluentAssertions;
using Moq;
using OpenTrivia.Api.Application.DTOs;
using OpenTrivia.Api.Application.Interfaces;
using OpenTrivia.Api.Application.Services;
using OpenTrivia.Api.Domain.Entities;

namespace OpenTrivia.Api.Tests.Application.Services;

public class QuizServiceTests
{
    private readonly Mock<ITriviaApiClient> _triviaApiClientMock;
    private readonly Mock<IQuizSessionCache> _sessionCacheMock;
    private readonly QuizService _sut;

    public QuizServiceTests()
    {
        _triviaApiClientMock = new Mock<ITriviaApiClient>();
        _sessionCacheMock = new Mock<IQuizSessionCache>();
        _sut = new QuizService(_triviaApiClientMock.Object, _sessionCacheMock.Object);
    }

    [Fact]
    public async Task CreateQuizSessionAsync_ShouldReturnSessionDto_WithShuffledOptions()
    {
        var questions = new List<TriviaQuestion>
        {
            new TriviaQuestion
            {
                Id = Guid.NewGuid(),
                Category = "Science",
                Type = "multiple",
                Difficulty = "easy",
                QuestionText = "What is water made of?",
                CorrectAnswer = "H2O",
                IncorrectAnswers = new List<string> { "CO2", "O2", "H2O2" }
            }
        };

        _triviaApiClientMock.Setup(x => x.GetQuestionsAsync(10, null, null, null))
            .ReturnsAsync(questions);

        var result = await _sut.CreateQuizSessionAsync(10, null, null, null);

        result.Should().NotBeNull();
        result.SessionId.Should().NotBeEmpty();
        result.Questions.Should().HaveCount(1);

        var questionDto = result.Questions.First();
        questionDto.QuestionText.Should().Be("What is water made of?");
        questionDto.Options.Should().HaveCount(4);
        questionDto.Options.Should().Contain("H2O");
        questionDto.Options.Should().Contain("CO2");
        questionDto.Options.Should().Contain("O2");
        questionDto.Options.Should().Contain("H2O2");

        _sessionCacheMock.Verify(x => x.SaveSession(It.Is<QuizSession>(s => s.Questions.Count == 1 && s.SessionId == result.SessionId)), Times.Once);
    }

    [Fact]
    public void CheckAnswers_WhenSessionNotFound_ShouldThrowException()
    {
        var request = new SubmitAnswersRequest { SessionId = Guid.NewGuid() };
        _sessionCacheMock.Setup(x => x.GetSession(request.SessionId)).Returns((QuizSession?)null);

        var act = () => _sut.CheckAnswers(request);

        act.Should().Throw<Exception>().WithMessage("Session not found or expired.");
    }

    [Fact]
    public void CheckAnswers_ShouldCalculateScoreCorrectly()
    {
        var sessionId = Guid.NewGuid();
        var q1Id = Guid.NewGuid();
        var q2Id = Guid.NewGuid();

        var session = new QuizSession
        {
            SessionId = sessionId,
            Questions = new List<TriviaQuestion>
            {
                new TriviaQuestion { Id = q1Id, QuestionText = "Q1", CorrectAnswer = "A1" },
                new TriviaQuestion { Id = q2Id, QuestionText = "Q2", CorrectAnswer = "A2" }
            }
        };

        _sessionCacheMock.Setup(x => x.GetSession(sessionId)).Returns(session);

        var request = new SubmitAnswersRequest
        {
            SessionId = sessionId,
            Answers = new Dictionary<Guid, string>
            {
                { q1Id, "A1" }, // Correct
                { q2Id, "Wrong" } // Incorrect
            }
        };

        var result = _sut.CheckAnswers(request);

        result.Should().NotBeNull();
        result.TotalQuestions.Should().Be(2);
        result.Score.Should().Be(1);
        result.Results.Should().HaveCount(2);

        var r1 = result.Results.First(r => r.QuestionId == q1Id);
        r1.IsCorrect.Should().BeTrue();
        r1.UserAnswer.Should().Be("A1");

        var r2 = result.Results.First(r => r.QuestionId == q2Id);
        r2.IsCorrect.Should().BeFalse();
        r2.UserAnswer.Should().Be("Wrong");
    }
}
