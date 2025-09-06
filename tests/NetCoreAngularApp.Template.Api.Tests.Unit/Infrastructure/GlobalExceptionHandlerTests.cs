using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NetCoreAngularApp.Template.Api.Infrastructure;
using NSubstitute;
using Xunit;

namespace NetCoreAngularApp.Template.Api.Tests.Unit.Infrastructure;

public class GlobalExceptionHandlerTests
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task TryHandleAsync_LogsErrorAndWritesProblemDetails()
    {
        // Arrange
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        var exception = new InvalidOperationException("Test exception");
        
        // Act
        var result = await handler.TryHandleAsync(
            context, 
            exception, 
            TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        responseStream.Seek(0, SeekOrigin.Begin);
        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            responseStream,
            _jsonOptions,
            TestContext.Current.CancellationToken);

        problemDetails.Should().NotBeNull();
        problemDetails!.Status.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be("Server failure");
        problemDetails.Type.Should().Be("https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1");
        problemDetails.Extensions.Should().ContainKey("traceId");
    }
}
