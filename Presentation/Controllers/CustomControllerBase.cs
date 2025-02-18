using System.Security.Claims;
using Application.Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public class CustomControllerBase(IHttpContextAccessor httpContextAccessor) : ControllerBase
{
    protected Guid GetUserId()
    {
        var userId = new Guid(
            httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return userId != Guid.Empty
            ? userId
            : throw new UnauthorizedAccessException();
    }

    protected static IActionResult Result<T>(Result<T> result)
    {
        return new ObjectResult(result)
        {
            StatusCode = (int)result.StatusCode
        };
    }
}