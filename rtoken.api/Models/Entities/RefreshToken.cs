namespace rtoken.api.Models.Entities
{
    public class RefreshToken
    {
        // private readonly IConfiguration _configuration;
        // public RefreshToken(IConfiguration configuration)
        // {
        //     _configuration = configuration;
        // }
        public int Id { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public string CreatedByIp { get; set; }
        public DateTime ExpiresAt { get; }
        public DateTime? RevokedAt { get; set; }
        public string RevokedByIp { get; set; }
        public string ReasonRevoked { get; set; }
        public string TokenSession { get; set; }
        public bool IsRevoked => RevokedAt == null ? false : true;
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}