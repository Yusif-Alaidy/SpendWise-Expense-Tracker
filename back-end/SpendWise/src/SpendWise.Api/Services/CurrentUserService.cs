// SpendWise.Api/Services/CurrentUserService.cs
using System.Security.Claims;
using SpendWise.Application.Common.Interfaces;

namespace SpendWise.Api.Services;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}