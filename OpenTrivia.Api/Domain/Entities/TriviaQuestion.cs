namespace OpenTrivia.Api.Domain.Entities;

public class TriviaQuestion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Category { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public List<string> IncorrectAnswers { get; set; } = new();
}
