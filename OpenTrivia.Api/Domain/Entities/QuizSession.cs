namespace OpenTrivia.Api.Domain.Entities;

public class QuizSession
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public List<TriviaQuestion> Questions { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
