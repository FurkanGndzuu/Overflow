var builder = DistributedApplication.CreateBuilder(args);

// Suppress ASPIRECERTIFICATES001 for evaluation purposes
#pragma warning disable ASPIRECERTIFICATES001
var keycloak = builder.AddKeycloak("keycloak", 6001).WithoutHttpsCertificate().WithDataVolume("keycloak-data");
#pragma warning restore ASPIRECERTIFICATES001


var postgres = builder.AddPostgres("postgres").WithDataVolume("postgres-data").WithPgAdmin();

var questionDb = postgres.AddDatabase("questionDb", "questionDb");


var questionService = builder.AddProject<Projects.QuestionService>("question-svc")
    .WithReference(keycloak)
    .WithReference(questionDb)
    .WaitFor(questionDb)
    .WaitFor(keycloak);




builder.Build().Run();
