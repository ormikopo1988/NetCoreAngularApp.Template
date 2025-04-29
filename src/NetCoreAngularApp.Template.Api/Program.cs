using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using NetCoreAngularApp.Template.Api;
using NetCoreAngularApp.Template.Application;
using NetCoreAngularApp.Template.Infrastructure;
using NetCoreAngularApp.Template.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddKeyVaultIfConfigured();

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
