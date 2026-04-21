using OpenTrivia.Api.Application.DTOs;
using OpenTrivia.Api.Application.Interfaces;
using OpenTrivia.Api.Domain.Entities;
using System.Runtime.InteropServices;

namespace OpenTrivia.Api.Application.Services;

public class QuizService : IQuizService
{
    private readonly ITriviaApiClient _triviaApiClient;
    private readonly IQuizSessionCache _sessionCache;

    public QuizService(ITriviaApiClient triviaApiClient, IQuizSessionCache sessionCache)
    {
        _triviaApiClient = triviaApiClient;
        _sessionCache = sessionCache;
    }

    public async Task<QuizSessionDto> CreateQuizSessionAsync(int amount = 10, int? category = null, string? difficulty = null, string? type = null)
    {
        var questions = await _triviaApiClient.GetQuestionsAsync(amount, category, difficulty, type);

        var session = new QuizSession
        {
            Questions = questions.ToList()
        };

        _sessionCache.SaveSession(session);

        var sessionDto = new QuizSessionDto
        {
            SessionId = session.SessionId,
            Questions = session.Questions.Select(q =>
            {
                var options = new List<string>(q.IncorrectAnswers) { q.CorrectAnswer };
                Random.Shared.Shuffle(CollectionsMarshal.AsSpan(options));

                return new QuestionDto
                {
                    Id = q.Id,
                    Category = q.Category,
                    Type = q.Type,
                    Difficulty = q.Difficulty,
                    QuestionText = q.QuestionText,
                    Options = options
                };
            }).ToList()
        };

        return sessionDto;
    }

    public QuizResultDto CheckAnswers(SubmitAnswersRequest request)
    {
        var session = _sessionCache.GetSession(request.SessionId);
        if (session == null)
        {
            throw new Exception("Session not found or expired.");
        }

        var result = new QuizResultDto
        {
            TotalQuestions = session.Questions.Count
        };

        foreach (var question in session.Questions)
        {
            var hasAnswer = request.Answers.TryGetValue(question.Id, out var userAnswer);
            userAnswer ??= string.Empty;

            var isCorrect = hasAnswer ? string.Equals(userAnswer, question.CorrectAnswer, StringComparison.OrdinalIgnoreCase) : false;

            if (isCorrect)
            {
                result.Score++;
            }

            result.Results.Add(new QuestionResultDto
            {
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                UserAnswer = userAnswer,
                CorrectAnswer = question.CorrectAnswer,
                IsCorrect = isCorrect
            });
        }

        return result;
    }
}