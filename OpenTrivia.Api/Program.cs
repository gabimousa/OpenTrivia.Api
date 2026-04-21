using Carter;
using OpenTrivia.Api.Application.Interfaces;
using OpenTrivia.Api.Application.Services;
using OpenTrivia.Api.Infrastructure.Cache;
using OpenTrivia.Api.Infrastructure.Clients;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

builder.Services.AddCarter();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ITriviaApiClient, OpenTriviaApiClient>();
builder.Services.AddSingleton<IQuizSessionCache, MemoryQuizSessionCache>();
builder.Services.AddScoped<IQuizService, QuizService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapCarter();

app.Run();