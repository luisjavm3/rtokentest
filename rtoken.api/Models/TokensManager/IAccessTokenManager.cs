namespace rtoken.api.Models.TokensManager
{
    public interface IAccessTokenManager
    {
        string GetAccessToken(int userId);
    }
}