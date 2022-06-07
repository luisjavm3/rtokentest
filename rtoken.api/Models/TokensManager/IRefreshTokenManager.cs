using rtoken.api.Models.Entities;

namespace rtoken.api.Models.TokensManager
{
    public interface IRefreshTokenManager
    {
        Task<RefreshToken> GetRefreshToken(string createdByIp, string tokenSession = null);
    }
}