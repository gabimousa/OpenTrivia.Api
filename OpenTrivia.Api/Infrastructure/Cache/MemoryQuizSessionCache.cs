using Microsoft.Extensions.Caching.Memory;
using OpenTrivia.Api.Application.Interfaces;
using OpenTrivia.Api.Domain.Entities;

namespace OpenTrivia.Api.Infrastructure.Cache;

public class MemoryQuizSessionCache : IQuizSessionCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _sessionDuration = TimeSpan.FromMinutes(30);

    public MemoryQuizSessionCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void SaveSession(QuizSession session)
    {
        _memoryCache.Set(session.SessionId, session, _sessionDuration);
    }

    public QuizSession? GetSession(Guid sessionId)
    {
        _memoryCache.TryGetValue(sessionId, out QuizSession? session);
        return session;
    }
}
