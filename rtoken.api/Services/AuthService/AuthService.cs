using Microsoft.EntityFrameworkCore;
using rtoken.api.Data;
using rtoken.api.DTOs.Auth;
using rtoken.api.DTOs.User;
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
            var userIp = GetClientIp();
            var foundUser = await _context.Users
                                .Include(u => u.RefreshTokens)
                                .FirstOrDefaultAsync(u => u.Username.ToLower().Equals(request.Username.ToLower()));

            if (foundUser == null || !PasswordUtils.MatchHashes(request.Password, foundUser.PasswordHash, foundUser.PasswordSalt))
                throw new AppException("Wrong credentials.");

            var accessToken = _aTokenManager.GetAccessToken(foundUser.Id);
            var refreshToken = await _rTokenManager.GetRefreshToken(userIp, foundUser);

            using (var transaction = _context.Database.BeginTransaction())
            {
                // Saves refresh token
                await _context.AddAsync(refreshToken);
                // Selects all user's RefreshTokens
                var userRToken2 = await _context.RefreshTokens
                                        .Include(rt => rt.User)
                                        .Where(rt => rt.User.Id == foundUser.Id)
                                        .ToListAsync();

                // First it revokes what it needs to. then removes.
                foreach (var rToken in userRToken2)
                {
                    // Revokes all user's refresh-tokens that are not revoked until now.
                    // It does not revoke the new one.
                    if (!rToken.IsRevoked && !rToken.TokenSession.Equals(refreshToken.Value))
                    {
                        rToken.RevokedAt = DateTime.UtcNow;
                        rToken.ReasonRevoked = "New Session Opened.";
                        rToken.RevokedByIp = userIp;
                    }

                    // Deletes all expired refresh-tokens.
                    if (rToken.IsExpired)
                        _context.RefreshTokens.Remove(rToken);
                }

                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            // Arranges login-response DTO.
            var loginResponse = new LoginResponse
            {
                Id = foundUser.Id,
                Username = foundUser.Username,
                AccessToken = accessToken,
                RequestToken = refreshToken.Value
            };

            response.Data = loginResponse;

            return response;
        }

        public async Task<ServiceResponse<LoginResponse>> RefreshToken(string rToken)
        {
            var response = new ServiceResponse<LoginResponse>();
            var storedRToken = await _context.RefreshTokens
                                .Include(rt => rt.User)
                                .FirstOrDefaultAsync(rt => rt.Value.Equals(rToken));

            if (storedRToken == null)
                throw new AppException("Invalid token.");

            var userIp = GetClientIp();
            var user = storedRToken.User;

            if (storedRToken.IsExpired && !storedRToken.IsRevoked)
            {
                var reasonRevoked = "User attempted to rotate a expired refresh-token.";
                _rTokenManager.RevokeToken(storedRToken, reasonRevoked, userIp);

                await _context.SaveChangesAsync();
                throw new AppException("Token expired.");
            }

            // Revokes all active tokens when attempting to rotate tokens with a revoked one.
            if (storedRToken.IsRevoked)
            {
                var tokens = await _context.RefreshTokens
                                .Include(t => t.User)
                                .Where(t => t.User.Id == storedRToken.User.Id).ToListAsync();

                foreach (var token in tokens)
                {
                    if (!token.IsRevoked)
                    {
                        var reasonRevoked = "Someone attempted to rotate a revoked refresh-token.";
                        _rTokenManager.RevokeToken(storedRToken, reasonRevoked, userIp);
                    }
                }

                await _context.SaveChangesAsync();
                throw new AppException("Token revoked");
            }

            // Arranges tokens
            var tokenSession = storedRToken.TokenSession;
            var accessToken = _aTokenManager.GetAccessToken(user.Id);
            // Rotates tokens
            var refreshToken = await _rTokenManager.GetRefreshToken(userIp, user, tokenSession);
            await _context.RefreshTokens.AddAsync(refreshToken);
            _rTokenManager.RevokeToken(storedRToken, $"Rotated by {refreshToken.Value}", userIp);

            await _context.SaveChangesAsync();

            // Arranges response
            var data = new LoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                AccessToken = accessToken,
                RequestToken = refreshToken.Value
            };
            response.Data = data;

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

        private int UserId
        {
            get
            {
                var user = (UserResponse)_httpContextAccessor.HttpContext.Items["User"];
                return user.Id;
            }
        }
    }
}