using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VaultlyBackend.Api.Models.BaseModels;

namespace VaultlyBackend.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string? message = null)
        {
            var traceId = HttpContext.TraceIdentifier;
            return Ok(ApiResponse<T>.Ok(data, traceId, message));
        }
        protected IActionResult Success(string? message = null)
        {
            var traceId = HttpContext.TraceIdentifier;
            return Ok(ApiResponse<object>.Ok(null, traceId, message));
        }

        protected IActionResult Fail(string message)
        {
            var traceId = HttpContext.TraceIdentifier;
            return BadRequest(ApiResponse<object>.Fail(message, traceId));
        }
    }
}
