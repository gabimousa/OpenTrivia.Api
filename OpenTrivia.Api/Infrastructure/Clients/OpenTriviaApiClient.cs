using OpenTrivia.Api.Application.Interfaces;
using OpenTrivia.Api.Domain.Entities;
using OpenTrivia.Api.Infrastructure.Models;
using System.Text;
using System.Text.Json;

namespace OpenTrivia.Api.Infrastructure.Clients;

public class OpenTriviaApiClient : ITriviaApiClient
{
    private readonly HttpClient _httpClient;

    public OpenTriviaApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://opentdb.com/");
    }

    public async Task<IEnumerable<TriviaQuestion>> GetQuestionsAsync(int amount = 10, int? category = null, string? difficulty = null, string? type = null)
    {
        var url = $"api.php?amount={amount}&encode=base64";

        if (category.HasValue)
            url += $"&category={category.Value}";
        if (!string.IsNullOrEmpty(difficulty))
            url += $"&difficulty={difficulty}";
        if (!string.IsNullOrEmpty(type))
            url += $"&type={type}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var apiResult = JsonSerializer.Deserialize<OpenTriviaResponse>(content);

        if (apiResult == null)
        {
            throw new Exception("Something went wront when retreiving questions");
        }

        if (apiResult.ResponseCode != 0)
        {
            throw new Exception(GetErrorMessage(apiResult.ResponseCode));
        }

        return apiResult.Results.Select(r => new TriviaQuestion
        {
            Id = Guid.NewGuid(),
            Category = DecodeBase64(r.Category),
            Type = DecodeBase64(r.Type),
            Difficulty = DecodeBase64(r.Difficulty),
            QuestionText = DecodeBase64(r.Question),
            CorrectAnswer = DecodeBase64(r.CorrectAnswer),
            IncorrectAnswers = r.IncorrectAnswers.Select(DecodeBase64).ToList()
        });
    }

    private string DecodeBase64(string base64EncodedData)
    {
        if (string.IsNullOrEmpty(base64EncodedData)) return string.Empty;
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }

    private string GetErrorMessage(int errorCode)
    {
        switch (errorCode)
        {
            case 1:
                return "Could not return results. The API doesn't have enough questions for your query.";

            case 2:
                return "Contains an invalid parameter. Arguements passed in aren't valid.";

            case 3:
                return "Session Token does not exist.";

            case 4:
                return "Session Token has returned all possible questions for the specified query. Resetting the Token is necessary.";

            case 5:
                return "Too many requests have occurred. Each IP can only access the API once every 5 seconds.";

            default:
                return "Unknown error while fetching questions";
        }
    }
}