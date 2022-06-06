namespace rtoken.api.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public Role Role { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}