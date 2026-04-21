namespace OpenTrivia.Api.Application.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
}