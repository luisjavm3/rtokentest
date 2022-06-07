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
        private readonly IRefreshTokenManager _rTokenManager;
        public AuthService(DataContext context, IRefreshTokenManager rTokenManager)
        {
            _context = context;
            _rTokenManager = rTokenManager;
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
    }
}