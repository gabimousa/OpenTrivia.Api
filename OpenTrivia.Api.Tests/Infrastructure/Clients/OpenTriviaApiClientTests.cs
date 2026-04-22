using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using Moq.Protected;
using OpenTrivia.Api.Infrastructure.Clients;
using OpenTrivia.Api.Infrastructure.Models;

namespace OpenTrivia.Api.Tests.Infrastructure.Clients;

public class OpenTriviaApiClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly OpenTriviaApiClient _sut;

    public OpenTriviaApiClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _sut = new OpenTriviaApiClient(_httpClient);
    }

    [Fact]
    public async Task GetQuestionsAsync_ShouldReturnDecodedQuestions_OnSuccess()
    {
        var apiResponse = new OpenTriviaResponse
        {
            ResponseCode = 0,
            Results = new List<OpenTriviaQuestion>
            {
                new OpenTriviaQuestion
                {
                    Category = Convert.ToBase64String(Encoding.UTF8.GetBytes("Science")),
                    Type = Convert.ToBase64String(Encoding.UTF8.GetBytes("multiple")),
                    Difficulty = Convert.ToBase64String(Encoding.UTF8.GetBytes("easy")),
                    Question = Convert.ToBase64String(Encoding.UTF8.GetBytes("What is H2O?")),
                    CorrectAnswer = Convert.ToBase64String(Encoding.UTF8.GetBytes("Water")),
                    IncorrectAnswers = new List<string>
                    {
                        Convert.ToBase64String(Encoding.UTF8.GetBytes("Oxygen")),
                        Convert.ToBase64String(Encoding.UTF8.GetBytes("Carbon"))
                    }
                }
            }
        };

        var jsonResponse = JsonSerializer.Serialize(apiResponse);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var result = await _sut.GetQuestionsAsync(10, null, null, null);

        result.Should().NotBeNull();
        result.Should().HaveCount(1);

        var question = result.First();
        question.Category.Should().Be("Science");
        question.Type.Should().Be("multiple");
        question.Difficulty.Should().Be("easy");
        question.QuestionText.Should().Be("What is H2O?");
        question.CorrectAnswer.Should().Be("Water");
        question.IncorrectAnswers.Should().HaveCount(2);
        question.IncorrectAnswers.Should().Contain("Oxygen");
        question.IncorrectAnswers.Should().Contain("Carbon");
    }

    [Fact]
    public async Task GetQuestionsAsync_ShouldThrowException_WhenResponseCodeIsNotZero()
    {
        var apiResponse = new OpenTriviaResponse
        {
            ResponseCode = 1, // Not enough questions
            Results = new List<OpenTriviaQuestion>()
        };

        var jsonResponse = JsonSerializer.Serialize(apiResponse);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var act = () => _sut.GetQuestionsAsync();

        await act.Should().ThrowAsync<Exception>().WithMessage("Could not return results. The API doesn't have enough questions for your query.");
    }
}
