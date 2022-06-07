using rtoken.api.DTOs.Auth;
using rtoken.api.Models;

namespace rtoken.api.Services.AuthService
{
    public interface IAuthService
    {
        Task<ServiceResponse<RegisterResponse>> Register(AuthRequest request);
    }
}