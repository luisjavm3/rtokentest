namespace rtoken.api.Models
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
    }
}