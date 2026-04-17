namespace OpenTrivia.Api.Endpoints.Questions;

public class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Questions", () =>
        {
        })
        .WithName("Questions");
    }
}