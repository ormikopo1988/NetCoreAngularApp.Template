using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NetCoreAngularApp.Template.Domain.Common;

public static class Results
{
    public static IResult Ok(
        EventId eventId = default)
    {
        return new Result<object>
        {
            EventId = eventId
        };
    }

    public static IResult<T> Ok<T>(
        T value, 
        EventId eventId = default)
    {
        return new Result<T>
        {
            Data = value,
            EventId = eventId
        };
    }

    public static IResult<T> Unspecified<T>(
        string message, 
        EventId eventId = default)
    {
        return Error<T>(
            ErrorCode.Unspecified,
            message, 
            eventId);
    }

    public static IResult<T> NotFound<T>(
        string message, 
        EventId eventId = default)
    {
        return Error<T>(
            ErrorCode.NotFound,
            message, 
            eventId);
    }

    public static IResult<T> ValidationFailed<T>(
        string message, 
        EventId eventId = default)
    {
        return Error<T>(
            ErrorCode.BadRequest,
            message, 
            eventId);
    }

    public static IResult<T> InternalServerError<T>(
        Exception ex, 
        EventId eventId = default)
    {
        return Error<T>(
            ErrorCode.InternalServerError,
            "Unhandled error", 
            eventId, 
            ex);
    }

    public static IResult<T> InternalServerError<T>(
        string message, 
        EventId eventId = default)
    {
        return Error<T>(
            ErrorCode.InternalServerError,
            message, 
            eventId);
    }

    public static IResult NotFound(
        string message, 
        EventId eventId = default)
    {
        return Error(
            ErrorCode.NotFound,
            message, 
            eventId);
    }

    public static IResult ValidationFailed(
        string message, 
        EventId eventId = default)
    {
        return Error(
            ErrorCode.BadRequest,
            message, 
            eventId);
    }

    public static IResult InternalServerError(
        Exception ex, 
        EventId eventId = default)
    {
        return Error(
            ErrorCode.InternalServerError,
            "Unhandled error", 
            eventId, 
            ex);
    }

    public static IResult InternalServerError(
        Exception ex, 
        string message,
        EventId eventId = default)
    {
        return Error(
            ErrorCode.InternalServerError,
            message, 
            eventId, 
            ex);
    }

    public static IResult BadGateway(
        string message, 
        EventId eventId = default)
    {
        return Error(
            ErrorCode.BadGateway,
            message, 
            eventId);
    }

    public static IResult Unauthorized(
        string message, 
        EventId eventId = default)
    {
        return Error(
            ErrorCode.Unauthorized,
            message, 
            eventId);
    }

    public static IResult Error(
        IResult result, 
        EventId? eventId = null)
    {
        return new Result<object>
        {
            EventId = eventId ?? result.EventId,
            Error = result.Error
        };
    }

    public static IResult<T> Error<T>(
        IResult result, 
        EventId? eventId = null)
    {
        return new Result<T>
        {
            EventId = eventId ?? result.EventId,
            Error = result.Error
        };
    }

    internal static IResult Error(
        ErrorCode code, 
        string message,
        EventId eventId, 
        Exception? ex = null)
    {
        return new Result<object>
        {
            EventId = eventId,
            Error = new Error
            {
                Code = code,
                Exception = ex,
                Message = message
            }
        };
    }

    internal static IResult<T> Error<T>(
        ErrorCode code, 
        string message,
        EventId eventId, 
        Exception? ex = null)
    {
        return new Result<T>
        {
            EventId = eventId,
            Error = new Error
            {
                Code = code,
                Exception = ex,
                Message = message
            }
        };
    }
}
