namespace OpenTrivia.Api.Application.DTOs;

public class SubmitAnswersRequest
{
    public Guid SessionId { get; set; }
    public Dictionary<Guid, string> Answers { get; set; } = new();
}
