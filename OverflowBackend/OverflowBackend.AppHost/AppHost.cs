using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Suppress ASPIRECERTIFICATES001 for evaluation purposes
#pragma warning disable ASPIRECERTIFICATES001
var keycloak = builder.AddKeycloak("keycloak", 6001).WithoutHttpsCertificate().WithDataVolume("keycloak-data");
#pragma warning restore ASPIRECERTIFICATES001


var postgres = builder.AddPostgres("postgres").WithDataVolume("postgres-data").WithPgAdmin();

var questionDb = postgres.AddDatabase("questionDb", "questionDb");

var typesenseApiKey = builder.AddParameter("typesense-api-key" , secret:true);

var typesense = builder.AddContainer("typesense", "typesense/typesense", "29.0")
    .WithVolume("typesense-data", "/data")
    .WithArgs("--data-dir" , "/data" , "--api-key" ,typesenseApiKey ,"--enable-cors")
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


builder.Build().Run();
