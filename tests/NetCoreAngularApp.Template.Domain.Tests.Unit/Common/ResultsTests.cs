using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NetCoreAngularApp.Template.Domain.Common;

namespace NetCoreAngularApp.Template.Domain.Tests.Unit.Common;

public class ResultsTests
{
    private static readonly EventId TestEventId = new(42, "TestEvent");

    [Fact]
    public void Ok_ShouldReturnSuccessResult_WhenCalledWithoutEventId()
    {
        // Act
        var result = Results.Ok();

        // Assert
        result.IsError.Should().BeFalse();
        result.Error.Should().BeNull();
        result.EventId.Should().Be(default(EventId));
    }

    [Fact]
    public void Ok_ShouldReturnSuccessResult_WhenCalledWithEventId()
    {
        // Act
        var result = Results.Ok(TestEventId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Error.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void OkOfT_ShouldReturnSuccessResultWithData_WhenCalledWithValue()
    {
        // Arrange
        const string value = "hello";

        // Act
        var result = Results.Ok(value, TestEventId);

        // Assert
        result.IsError.Should().BeFalse();
        result.Data.Should().Be(value);
        result.Error.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void OkOfT_ShouldDefaultEventId_WhenNotProvided()
    {
        // Act
        var result = Results.Ok(123);

        // Assert
        result.IsError.Should().BeFalse();
        result.Data.Should().Be(123);
        result.Error.Should().BeNull();
        result.EventId.Should().Be(default(EventId));
    }

    [Fact]
    public void OkOfT_ShouldAllowNullData_WhenValueIsNull()
    {
        // Act
        var result = Results.Ok<string?>(null);

        // Assert
        result.IsError.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void UnspecifiedOfT_ShouldReturnUnspecifiedErrorResult_WhenCalled()
    {
        // Arrange
        const string message = "Something happened";

        // Act
        var result = Results.Unspecified<string>(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.Unspecified);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void NotFoundOfT_ShouldReturnNotFoundErrorResult_WhenCalled()
    {
        // Arrange
        const string message = "Resource missing";

        // Act
        var result = Results.NotFound<string>(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void ValidationFailedOfT_ShouldReturnBadRequestErrorResult_WhenCalled()
    {
        // Arrange
        const string message = "Bad input";

        // Act
        var result = Results.ValidationFailed<string>(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.BadRequest);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void InternalServerErrorOfT_ShouldReturnErrorWithUnhandledMessage_WhenCalledWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("boom");

        // Act
        var result = Results.InternalServerError<string>(exception, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.InternalServerError);
        result.Error.Message.Should().Be("Unhandled error");
        result.Error.Exception.Should().BeSameAs(exception);
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void InternalServerErrorOfT_ShouldReturnErrorWithMessage_WhenCalledWithMessage()
    {
        // Arrange
        const string message = "Custom server error";

        // Act
        var result = Results.InternalServerError<string>(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.InternalServerError);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void NotFound_ShouldReturnNotFoundErrorResult_WhenCalled()
    {
        // Arrange
        const string message = "Missing";

        // Act
        var result = Results.NotFound(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.NotFound);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void ValidationFailed_ShouldReturnBadRequestErrorResult_WhenCalled()
    {
        // Arrange
        const string message = "Invalid";

        // Act
        var result = Results.ValidationFailed(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.BadRequest);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void InternalServerError_ShouldReturnErrorWithUnhandledMessage_WhenCalledWithException()
    {
        // Arrange
        var exception = new InvalidOperationException("oops");

        // Act
        var result = Results.InternalServerError(exception, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.InternalServerError);
        result.Error.Message.Should().Be("Unhandled error");
        result.Error.Exception.Should().BeSameAs(exception);
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void InternalServerError_ShouldUseProvidedMessage_WhenCalledWithExceptionAndMessage()
    {
        // Arrange
        var exception = new InvalidOperationException("oops");
        const string message = "Custom failure";

        // Act
        var result = Results.InternalServerError(exception, message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.InternalServerError);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeSameAs(exception);
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void BadGateway_ShouldReturnBadGatewayErrorResult_WhenCalled()
    {
        // Arrange
        const string message = "Upstream is down";

        // Act
        var result = Results.BadGateway(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.BadGateway);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void Unauthorized_ShouldReturnUnauthorizedErrorResult_WhenCalled()
    {
        // Arrange
        const string message = "Access denied";

        // Act
        var result = Results.Unauthorized(message, TestEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.Unauthorized);
        result.Error.Message.Should().Be(message);
        result.Error.Exception.Should().BeNull();
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void Error_ShouldCopyErrorAndInheritEventId_WhenNoOverrideProvided()
    {
        // Arrange
        var source = Results.NotFound("Original", TestEventId);

        // Act
        var result = Results.Error(source);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().BeSameAs(source.Error);
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void Error_ShouldUseOverrideEventId_WhenProvided()
    {
        // Arrange
        var source = Results.NotFound("Original", TestEventId);
        var overrideEventId = new EventId(99, "Override");

        // Act
        var result = Results.Error(source, overrideEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().BeSameAs(source.Error);
        result.EventId.Should().Be(overrideEventId);
    }

    [Fact]
    public void ErrorOfT_ShouldCopyErrorAndInheritEventId_WhenNoOverrideProvided()
    {
        // Arrange
        var source = Results.NotFound("Original", TestEventId);

        // Act
        var result = Results.Error<string>(source);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().BeSameAs(source.Error);
        result.EventId.Should().Be(TestEventId);
    }

    [Fact]
    public void ErrorOfT_ShouldUseOverrideEventId_WhenProvided()
    {
        // Arrange
        var source = Results.NotFound("Original", TestEventId);
        var overrideEventId = new EventId(99, "Override");

        // Act
        var result = Results.Error<string>(source, overrideEventId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Data.Should().BeNull();
        result.Error.Should().BeSameAs(source.Error);
        result.EventId.Should().Be(overrideEventId);
    }

    [Fact]
    public void ErrorOfT_ShouldPropagateExceptionFromSource_WhenSourceHasException()
    {
        // Arrange
        var exception = new InvalidOperationException("inner");
        var source = Results.InternalServerError(exception, TestEventId);

        // Act
        var result = Results.Error<int>(source);

        // Assert
        result.IsError.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(ErrorCode.InternalServerError);
        result.Error.Exception.Should().BeSameAs(exception);
        result.EventId.Should().Be(TestEventId);
    }
}
