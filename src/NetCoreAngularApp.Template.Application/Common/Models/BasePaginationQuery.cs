namespace NetCoreAngularApp.Template.Application.Common.Models;

public class BasePaginationQuery
{
    public required int Page { get; init; } = 1;

    public required int PageSize { get; init; } = 10;
}
