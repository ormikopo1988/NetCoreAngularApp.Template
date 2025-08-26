using System;

namespace NetCoreAngularApp.Template.Domain.Common;

public class Error
{
    public ErrorCode Code { get; set; }

    public Exception? Exception { get; set; }

    public required string Message { get; set; }
}
