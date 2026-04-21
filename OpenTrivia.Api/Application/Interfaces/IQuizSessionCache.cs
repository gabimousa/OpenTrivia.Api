using OpenTrivia.Api.Domain.Entities;

namespace OpenTrivia.Api.Application.Interfaces;

public interface IQuizSessionCache
{
    void SaveSession(QuizSession session);
    QuizSession? GetSession(Guid sessionId);
}
