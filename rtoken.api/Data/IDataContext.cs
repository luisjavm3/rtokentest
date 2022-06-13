using Microsoft.EntityFrameworkCore;
using rtoken.api.Models.Entities;

namespace rtoken.api.Data
{
    public interface IDataContext
    {
        DbSet<User> Users { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}