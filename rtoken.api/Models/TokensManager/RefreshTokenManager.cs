using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using rtoken.api.Data;
using rtoken.api.Models.Entities;

namespace rtoken.api.Models.TokensManager
{
    public class RefreshTokenManager : IRefreshTokenManager
    {
        private readonly DataContext _context;
        public RefreshTokenManager(DataContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetRefreshToken(string createdByIp, User user, string tokenSession = null)
        {
            var value = await GetValue();
            tokenSession ??= value;

            // createdAt and ExpiresAt are assigned on RefreshToken entity.
            // Remaining properties stay as null.
            var rToken = new RefreshToken
            {
                Value = value,
                CreatedByIp = createdByIp,
                TokenSession = tokenSession,
                User = user
            };

            return rToken;

            async Task<string> GetValue()
            {
                var value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                var valueExists = await _context.RefreshTokens.AnyAsync(rt => rt.Value.ToLower().Equals(value.ToLower()));

                if (!valueExists)
                    return value;

                return await GetValue();
            }
        }

        public void RevokeToken(RefreshToken token, string reasonRevoked, string ip)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.ReasonRevoked = reasonRevoked;
            token.RevokedByIp = ip;
        }
    }
}