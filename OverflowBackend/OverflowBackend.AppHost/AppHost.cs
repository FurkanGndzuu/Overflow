using Aspire.Hosting;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var compose = builder.AddDockerComposeEnvironment("production")
    .WithDashboard(config => config.WithHostPort(8080));

// Suppress ASPIRECERTIFICATES001 for evaluation purposes
#pragma warning disable ASPIRECERTIFICATES001
var keycloak = builder.AddKeycloak("keycloak", 6001).WithoutHttpsCertificate().WithDataVolume("keycloak-data")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_HOSTNAME_STRICT", "false")
    .WithRealmImport("../infra/realms")
    .WithEndpoint(6001, 8080, "keycloak", isExternal: true);
#pragma warning restore ASPIRECERTIFICATES001


var postgres = builder.AddPostgres("postgres").WithDataVolume("postgres-data").WithPgAdmin();

var questionDb = postgres.AddDatabase("questionDb", "questionDb");

var typesenseApiKey = builder.AddParameter("typesense-api-key" , secret:true);

var typesense = builder.AddContainer("typesense", "typesense/typesense", "29.0")
    .WithVolume("typesense-data", "/data")
    .WithEnvironment("TYPESENSE_API_KEY", typesenseApiKey)
    .WithEnvironment("TYPESENSE_DATA_DIR", "/data")
    .WithEnvironment("TYPESENSE_ENABLE_CORS" , "true")
    .WithHttpEndpoint(8108, 8108, name: "typesense");

var typeSenseContainer = typesense.GetEndpoint("typesense");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithDataVolume("rabbitmq-data")
    .WithManagementPlugin(15672);;

var questionService = builder.AddProject<Projects.QuestionService>("question-svc")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WaitFor(questionDb)
    .WaitFor(keycloak).
    WithReference(rabbitmq);

var searchService = builder.AddProject<Projects.SearchService>("search-svc")
    .WithReference(typeSenseContainer)
    .WithEnvironment("typesense-api-key", typesenseApiKey)
    .WaitFor(typesense)
    .WithReference(rabbitmq);

var gateway = builder.AddYarp("overflow-proxy").WithConfiguration(yarpBuilder =>
{
    yarpBuilder.AddRoute("/search/{**catch-all}", searchService);
    yarpBuilder.AddRoute("/questions/{**catch-all}", questionService);
    yarpBuilder.AddRoute("/tags/{**catch-all}", questionService);

}).WithEnvironment("ASPNETCORE_URLS", "http://*:8001")
    .WithEndpoint(port: 8001, scheme: "http", targetPort: 8001, name: "gateway", isExternal: true);

if (!builder.Environment.IsDevelopment())
{
   var nginxproxy = builder.AddContainer("ngnix-proxy", "nginxproxy/nginx-proxy", "1.9")
        .WithEndpoint(80, 80, isExternal: true)
        .WithVolume("/var/run/docker.sock", "/tmp/docker.sock:ro" ,true);
}


builder.Build().Run();
