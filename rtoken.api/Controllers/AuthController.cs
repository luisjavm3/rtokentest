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
    }
}