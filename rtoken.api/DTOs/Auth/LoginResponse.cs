using System.Text.Json.Serialization;

namespace rtoken.api.DTOs.Auth
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string RequestToken { get; set; }
    }
}