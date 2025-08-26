using Microsoft.Extensions.Logging;

namespace NetCoreAngularApp.Template.Domain.Common;

public class Result<T> : IResult<T>
{
    public bool IsError => Error != null;

    public T? Data { get; set; }

    public EventId EventId { get; set; }

    public Error? Error { get; set; }
}
