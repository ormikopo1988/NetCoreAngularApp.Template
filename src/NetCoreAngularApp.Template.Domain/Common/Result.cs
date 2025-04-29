namespace NetCoreAngularApp.Template.Domain.Common;

public class Result<T>
{
    public T Data { get; }

    public Error? Error { get; }

    public bool IsSuccess => Error is null;

    public bool IsFailure => !IsSuccess;

    private Result(T data, Error? error)
    {
        Data = data;
        Error = error;
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(data, default!);
    }

    public static Result<T> Failure(
        ErrorCode errorCode,
        string errorMessage)
    {
        return new Result<T>(
            default!,
            new Error
            {
                ErrorCode = errorCode,
                Message = errorMessage
            });
    }

    public static Result<T> Failure(
        Error error)
    {
        return new Result<T>(
            default!,
            error);
    }
}

public class Error
{
    public ErrorCode ErrorCode { get; set; }

    public string Message { get; set; } = default!;
}

public enum ErrorCode
{
    Unspecified = 0
}
