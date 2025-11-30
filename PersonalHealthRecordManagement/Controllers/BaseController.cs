using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PersonalHealthRecordManagement.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected ActionResult<T> UnauthorizedResponse<T>(string message = "Unauthorized")
        {
            return Unauthorized(new { error = message });
        }

        protected ActionResult<T> NotFoundResponse<T>(string message = "Resource not found")
        {
            return NotFound(new { error = message });
        }

        protected ActionResult<T> BadRequestResponse<T>(string message)
        {
            return BadRequest(new { error = message });
        }
    }
}

