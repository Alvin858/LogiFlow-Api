using System.Security.Claims;
using LogiFlow.Application.Services;

namespace LogiFlow.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(value, out var id)) throw new UnauthorizedException("User identity is invalid.");
        return id;
    }
}
