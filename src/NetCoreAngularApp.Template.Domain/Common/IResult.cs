using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace NetCoreAngularApp.Template.Domain.Common;

public interface IResult
{
    [MemberNotNullWhen(true, nameof(Error))]
    bool IsError { get; }

    EventId EventId { get; }

    Error? Error { get; }
}

public interface IResult<out T> : IResult
{
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Data))]
    new bool IsError { get; }

    new Error? Error { get; }

    T? Data { get; }
}
