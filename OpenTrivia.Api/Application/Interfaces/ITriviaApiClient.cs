using OpenTrivia.Api.Domain.Entities;

namespace OpenTrivia.Api.Application.Interfaces;

public interface ITriviaApiClient
{
    Task<IEnumerable<TriviaQuestion>> GetQuestionsAsync(int amount = 10, int? category = null, string? difficulty = null, string? type = null);
}
