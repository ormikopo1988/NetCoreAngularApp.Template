using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NetCoreAngularApp.Template.Api.Extensions;
using NetCoreAngularApp.Template.Domain.Common;

namespace NetCoreAngularApp.Template.Api.Tests.Unit.Extensions;

public class ResultExtensionsTests
{
    [Fact]
    public async Task AsActionResult_WhenSuccessful_ShouldReturnNoContent()
    {
        // Arrange
        var successResult = Results.Ok();
        var resultTask = Task.FromResult(successResult);

        // Act
        var actionResult = await resultTask.AsActionResult();

        // Assert
        actionResult.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AsActionResult_WhenError_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error { Code = ErrorCode.NotFound, Message = "Not found" };
        var errorResult = Results.Error(new Result<object> { Error = error });
        var resultTask = Task.FromResult(errorResult);

        // Act
        var actionResult = await resultTask.AsActionResult();

        // Assert
        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Title.Should().Be(ErrorCode.NotFound.ToString());
        problemDetails.Detail.Should().Be("Not found");
        problemDetails.Status.Should().Be((int)ErrorCode.NotFound);
        problemDetails.Extensions.Should().ContainKey("eventId");
        problemDetails.Extensions.Should().ContainKey("traceId");
    }

    [Fact]
    public async Task AsActionResultGeneric_WhenSuccessful_ShouldReturnOkWithData()
    {
        // Arrange
        var data = "Test Data";
        var successResult = Results.Ok(data);
        var resultTask = Task.FromResult(successResult);

        // Act
        var actionResult = await resultTask.AsActionResult();

        // Assert
        var okResult = actionResult.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(data);
    }

    [Fact]
    public async Task AsActionResultGeneric_WhenError_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error { Code = ErrorCode.BadRequest, Message = "Invalid input" };
        var errorResult = Results.Error<string>(new Result<string> { Error = error });
        var resultTask = Task.FromResult(errorResult);

        // Act
        var actionResult = await resultTask.AsActionResult();

        // Assert
        var objectResult = actionResult.Result.Should().BeOfType<ObjectResult>().Subject;
        var problemDetails = objectResult.Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Title.Should().Be(ErrorCode.BadRequest.ToString());
        problemDetails.Detail.Should().Be("Invalid input");
        problemDetails.Status.Should().Be((int)ErrorCode.BadRequest);
        problemDetails.Extensions.Should().ContainKey("eventId");
        problemDetails.Extensions.Should().ContainKey("traceId");
    }
}
