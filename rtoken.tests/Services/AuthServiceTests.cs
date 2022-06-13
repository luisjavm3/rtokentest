using rtoken.api.Data;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using rtoken.api.Models.Entities;
using System.Linq;

namespace rtoken.tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public void No_Name()
        {
            var mockDbSet = new Mock<DbSet<RefreshToken>>();
            mockDbSet.As<IQueryable<RefreshToken>>().Setup(m => m.Provider).Returns(data.Provider);
            mockDbSet.As<IQueryable<RefreshToken>>().Setup(m => m.Expression).Returns(data.Expression);
            mockDbSet.As<IQueryable<RefreshToken>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockDbSet.As<IQueryable<RefreshToken>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            // }
        }
    }