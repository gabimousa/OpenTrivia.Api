using Carter;
using Microsoft.AspNetCore.Mvc;
using OpenTrivia.Api.Application.DTOs;
using OpenTrivia.Api.Application.Interfaces;

namespace OpenTrivia.Api.Endpoints;

public class TriviaEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/trivia").WithTags("Trivia");

        group.MapGet("/questions", async (
            [FromServices] IQuizService quizService,
            [FromQuery] int amount = 10,
            [FromQuery] int? category = null,
            [FromQuery] string? difficulty = null,
            [FromQuery] string? type = null) =>
        {
            try
            {
                var session = await quizService.CreateQuizSessionAsync(amount, category, difficulty, type);
                return Results.Ok(session);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message);
            }
        });

        group.MapPost("/checkanswers", (
            [FromServices] IQuizService quizService,
            [FromBody] SubmitAnswersRequest request) =>
        {
            try
            {
                var result = quizService.CheckAnswers(request);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }
}