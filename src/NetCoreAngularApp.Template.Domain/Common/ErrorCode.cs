namespace NetCoreAngularApp.Template.Domain.Common;

public enum ErrorCode
{
    Unspecified = 0,
    BadRequest = 400,
    Unauthorized = 401,
    NotFound = 404,
    Conflict = 409,
    InternalServerError = 500,
    BadGateway = 502,
}
