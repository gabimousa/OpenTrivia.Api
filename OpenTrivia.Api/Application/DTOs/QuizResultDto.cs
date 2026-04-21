namespace OpenTrivia.Api.Application.DTOs;

public class QuizResultDto
{
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public List<QuestionResultDto> Results { get; set; } = new();
}

public class QuestionResultDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
