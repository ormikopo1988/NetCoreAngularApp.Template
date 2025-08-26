using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCoreAngularApp.Template.Domain.Common;

namespace NetCoreAngularApp.Template.Api.Extensions;

internal static class ResultExtensions
{
    public static async Task<ActionResult<T>> 
        AsActionResult<T>(
            this Task<IResult<T>> resultTask)
    {
        var result = await resultTask;

        if (!result.IsError)
        {
            return new OkObjectResult(result.Data);
        }

        var details = new ProblemDetails
        {
            Title = result.Error.Code.ToString(),
            Detail = result.Error.Message,
            Status = (int)result.Error.Code,
            Extensions = new Dictionary<string, object?>
            {
                ["eventId"] = result.EventId.Id,
                ["traceId"] = Activity.Current?.Id?.ToString()
            }
        };

        return new ObjectResult(details)
        {
            StatusCode = details.Status
        };
    }

    public static async Task<ActionResult> 
        AsActionResult(
            this Task<IResult> resultTask)
    {
        var result = await resultTask;

        if (!result.IsError)
        {
            return new NoContentResult();
        }

        var details = new ProblemDetails
        {
            Title = result.Error.Code.ToString(),
            Detail = result.Error.Message,
            Status = (int)result.Error.Code,
            Extensions = new Dictionary<string, object?>
            {
                ["eventId"] = result.EventId.Id,
                ["traceId"] = Activity.Current?.Id?.ToString()
            }
        };

        return new ObjectResult(details)
        {
            StatusCode = details.Status
        };
    }
}
