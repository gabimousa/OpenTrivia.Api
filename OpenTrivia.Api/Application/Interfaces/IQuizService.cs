using OpenTrivia.Api.Application.DTOs;

namespace OpenTrivia.Api.Application.Interfaces;

public interface IQuizService
{
    QuizResultDto CheckAnswers(SubmitAnswersRequest request);
    Task<QuizSessionDto> CreateQuizSessionAsync(int amount = 10, int? category = null, string? difficulty = null, string? type = null);
}