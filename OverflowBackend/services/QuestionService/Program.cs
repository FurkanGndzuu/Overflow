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

builder.Services.AddAuthentication().AddKeycloakJwtBearer("keycloak" , "overflow" , options =>
{
    options.Audience = "overflow";
    options.RequireHttpsMetadata = false;
});
builder.Services.AddOpenTelemetry().WithTracing(conf =>
{
    conf.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddSource("Wolverine");
});

builder.Host.UseWolverine(config =>
{
    config.UseRabbitMqUsingNamedConnection("messaging");
    config.PublishAllMessages().ToRabbitExchange("questions");
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


