using System.Collections.Generic;

namespace NetCoreAngularApp.Template.Application.Common.Models;

public class PaginatedList<TResponse>
{
    public required IEnumerable<TResponse> Items { get; init; } = [];

    public required int PageSize { get; init; }

    public required int Page { get; init; } = 1;

    public required int Total { get; init; } = 10;

    public bool HasNextPage => Total > Page * PageSize;
}
