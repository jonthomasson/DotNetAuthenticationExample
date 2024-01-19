using DotNetAuthentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DotNetAuthentication.Helpers;

public class AuthorizeRoles : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private Role[] _checkRoles { get; set; }
    public AuthorizeRoles(params Role[] roles)
    {
        _checkRoles = roles;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var userRole = context.HttpContext.User
                .FindFirst(ClaimTypes.Role).Value;


        if (!_checkRoles.Any(x => userRole.ToString() == x.ToString()))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
