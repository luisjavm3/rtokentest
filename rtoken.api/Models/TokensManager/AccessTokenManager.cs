using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace rtoken.api.Models.TokensManager
{
    public class AccessTokenManager : IAccessTokenManager
    {
        private readonly IConfiguration _config;
        public AccessTokenManager(IConfiguration config)
        {
            _config = config;
        }

        public string GetAccessToken(int userId)
        {
            var encodedKey = System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:token_secrect").Value);
            var signingKey = new SymmetricSecurityKey(encodedKey);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature);
            var claims = new[]{
                new Claim(type:"id", value: userId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config.GetSection("AppSettings:access_token_lifetime").Value)),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredentials,

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}