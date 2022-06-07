using rtoken.api.DTOs.Auth;
using rtoken.api.Models;

namespace rtoken.api.Services.AuthService
{
    public interface IAuthService
    {
        Task Register(AuthRequest request);
    }
}