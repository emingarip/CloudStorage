using AuthService.Application.Commands.Login;
using AuthService.Application.Commands.RefreshToken;
using AuthService.Application.Commands.Register;
using AuthService.Application.Queries.GetUserById;
using AuthService.Application.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private string GetIpAddress()
        {
            // Get client IP address from the request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "127.0.0.1";
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            // Get client IP address
            command.IpAddress = GetIpAddress();

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            // Get client IP address
            command.IpAddress = GetIpAddress();

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            // Get client IP address
            command.IpAddress = GetIpAddress();

            var result = await _mediator.Send(command);

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            {
                return Unauthorized();
            }

            var query = new GetUserByIdQuery { UserId = id };
            var result = await _mediator.Send(query);

            if (result.IsFailure)
            {
                return NotFound(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());

            if (result.IsFailure)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(result.Value);
        }
    }
}