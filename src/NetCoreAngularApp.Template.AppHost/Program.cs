var builder = DistributedApplication.CreateBuilder(args);

// Automatically provision an Application Insights resource
var insights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights("NetCoreAngularAppTemplateAppInsights")
    : builder.AddConnectionString("NetCoreAngularAppTemplateAppInsights", "APPLICATIONINSIGHTS_CONNECTION_STRING");

var databaseName = "NetCoreAngularAppTemplateDb";

var postgresDb = builder
    .AddPostgres("PostgresDB")
    // Set the name of the default database to auto-create on container startup.
    .WithEnvironment("POSTGRES_DB", databaseName)
    .AddDatabase("DefaultConnection", databaseName);

var netCoreAngularAppTemplateApi = builder
    .AddProject<Projects.NetCoreAngularApp_Template_Api>("NetCoreAngularAppTemplateApi")
    .WithReference(postgresDb)
    .WaitFor(postgresDb)
    .WithReference(insights)
    .WithExternalHttpEndpoints();

// Angular: npm run start
if (builder.ExecutionContext.IsRunMode)
{
    builder.AddJavaScriptApp(
        "NetCoreAngularAppTemplateClient", "../NetCoreAngularApp.Template.Client", "start")
        .WithNpm()
        .WithReference(netCoreAngularAppTemplateApi)
        .WaitFor(netCoreAngularAppTemplateApi)
        .WithHttpEndpoint(env: "PORT");
}

await builder.Build().RunAsync();
