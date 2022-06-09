using Microsoft.EntityFrameworkCore;
using rtoken.api.Data;
using rtoken.api.DTOs.Auth;
using rtoken.api.Models;
using rtoken.api.Models.Entities;
using rtoken.api.Models.TokensManager;
using rtoken.api.Utils;

namespace rtoken.api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IAccessTokenManager _aTokenManager;
        private readonly IRefreshTokenManager _rTokenManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(DataContext context, IAccessTokenManager aTokenManager, IRefreshTokenManager rTokenManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _aTokenManager = aTokenManager;
            _rTokenManager = rTokenManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Register(AuthRequest request)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username.ToLower().Equals(request.Username));

            if (userExists)
                throw new AppException("User already exists.");

            PasswordUtils.GetPasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Username = request.Username,
                Role = Role.User,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<ServiceResponse<LoginResponse>> Login(AuthRequest request)
        {
            var response = new ServiceResponse<LoginResponse>();
            var foundUser = await _context.Users
                                .Include(u => u.RefreshTokens)
                                .FirstOrDefaultAsync(u => u.Username.ToLower().Equals(request.Username));

            if (foundUser == null || PasswordUtils.MatchHashes(request.Password, foundUser.PasswordHash, foundUser.PasswordSalt))
                throw new AppException("Wrong credentials.");

            var accessToken = _aTokenManager.GetAccessToken(foundUser.Id);
            var refreshToken = _rTokenManager.GetRefreshToken(GetClientIp());

            return response;
        }

        private string GetClientIp()
        {
            var ipComesInHeaders = _httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For");

            return ipComesInHeaders
                ?
                    _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"]
                :
                    _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}