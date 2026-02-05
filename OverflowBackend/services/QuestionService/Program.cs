using Common;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuestionService.Contexts;
using QuestionService.Services;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.RabbitMQ;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddServiceDefaults();
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<TagService>();

var connString = builder.Configuration.GetConnectionString("questionDb");

builder.Services.AddDbContext<QuestionDbContext>(options =>
{
    options.UseNpgsql(connString);
}, optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddKeycloakAuthentication();

await builder.UseWolverineRabbitMqAsync(conf =>
{
    conf.PublishAllMessages().ToRabbitExchange("questions");
    conf.ApplicationAssembly = typeof(Program).Assembly;
});


builder.Services.AddAuthorization();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<QuestionDbContext>();
dbContext.Database.Migrate();
dbContext.SaveChanges();


app.MapControllers();



app.Run();


