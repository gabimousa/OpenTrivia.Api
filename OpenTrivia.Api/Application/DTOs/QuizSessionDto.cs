namespace OpenTrivia.Api.Application.DTOs;

public class QuizSessionDto
{
    public Guid SessionId { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}