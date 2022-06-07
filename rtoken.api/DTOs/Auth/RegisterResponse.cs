using System.Text.Json.Serialization;

namespace rtoken.api.DTOs.Auth
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string token { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }

    }
}