using rtoken.api.DTOs.Auth;
using rtoken.api.Models;

namespace rtoken.api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        public Task<ServiceResponse<RegisterResponse>> Register(AuthRequest request)
        {
            throw new NotImplementedException();
        }
    }
}