using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using rtoken.api.DTOs.Auth;
using rtoken.api.Services.AuthService;

namespace rtoken.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(AuthRequest request)
        {
            await _authService.Register(request);
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(AuthRequest request)
        {
            var result = await _authService.Login(request);
            SetCookieOnClient(result.Data.RequestToken);
            return Ok(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            var rToken = Request.Cookies["refreshToken"];

            if (rToken == null)
                return BadRequest("No refresh-token provided.");

            var result = await _authService.RefreshToken(rToken);
            SetCookieOnClient(result.Data.RequestToken);
            return Ok(result);
        }

        [HttpPost("RevokeToken")]
        public async Task<ActionResult> RevokeRToken()
        {
            var rToken = Request.Cookies["refreshToken"];

            if (rToken == null)
                return BadRequest("No refresh-token provided.");

            await _authService.RevokeRToken(rToken);
            Response.Cookies.Delete("refreshToken");

            return Ok();
        }

        private void SetCookieOnClient(string rTokenValue)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1)
            };

            Response.Cookies.Append("refreshToken", rTokenValue, cookieOptions);
        }
    }
}