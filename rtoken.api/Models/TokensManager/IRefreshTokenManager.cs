using rtoken.api.Models.Entities;

namespace rtoken.api.Models.TokensManager
{
    public interface IRefreshTokenManager
    {
        Task<RefreshToken> GetRefreshToken(string createdByIp, User user, string tokenSession = null);
        void RevokeToken(RefreshToken token, string reasonRevoked, string ip);
    }
}