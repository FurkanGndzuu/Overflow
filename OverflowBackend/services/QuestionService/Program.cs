using Microsoft.EntityFrameworkCore;
using QuestionService.Contexts;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

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


