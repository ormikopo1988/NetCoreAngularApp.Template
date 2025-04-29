using NetCoreAngularApp.Template.Application.Common.Interfaces;

namespace NetCoreAngularApp.Template.Infrastructure.Identity;

public class CurrentUser : IUser
{
    public string? Id { get; set; }
}
