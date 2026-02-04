using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SearchService.Models;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Typesense;
using Typesense.Setup;
using Wolverine;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

var typesenseUri = builder.Configuration["services:typesense:typesense:0"];
if(string.IsNullOrEmpty(typesenseUri))
    throw new InvalidOperationException("Typesense URI is not configured.");

var typesenseApiKey = builder.Configuration["typesense-api-key"];
if(string.IsNullOrEmpty(typesenseApiKey))
    throw new InvalidOperationException("Typesense API key is not configured.");


Uri uri = new(typesenseUri);
builder.Services.AddTypesenseClient(config =>
{
    config.Nodes = new List<Node>
    {
        new Node(uri.Host , uri.Port.ToString() , uri.Scheme)
    };
    config.ApiKey = typesenseApiKey;
});

builder.Services.AddOpenTelemetry().WithTracing(conf =>
{
    conf.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddSource("Wolverine");
});

builder.Host.UseWolverine(config =>
{
    config.UseRabbitMqUsingNamedConnection("messaging").AutoProvision();
    config.ListenToRabbitQueue("questions.search", cfg =>
    {
        cfg.BindExchange("questions");
    });
});


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/search", async (string query , ITypesenseClient client) =>
{
    string? tag = null;
    var tagMatch = Regex.Match(query, @"\[(.*?)\]");

    if (tagMatch.Success)
    {
        tag = tagMatch.Groups[1].Value;
        query = query.Replace(tagMatch.Value, "").Trim();
    }

    var searchParams = new SearchParameters(query, "title,content");

    if (!string.IsNullOrWhiteSpace(tag))
    {
      
        searchParams.FilterBy = $"tags:=[{tag}]";
    }

    try
    {
        var result = await client.Search<SearchQuestion>("questions", searchParams);
        return Results.Ok(result.Hits.Select(hit => hit.Document));
    }
    catch (Exception ex)
    {
        return Results.Problem("Typesense search failed: " + ex.Message);
    }
});

using var scope = app.Services.CreateScope();
var typesenseClient = scope.ServiceProvider.GetRequiredService<ITypesenseClient>();
await SearchService.Data.SearchInitializer.EnsureIndexExists(typesenseClient);



app.Run();

