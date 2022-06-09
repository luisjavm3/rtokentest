using rtoken.api.Data;
using Xunit;
using Moq;

namespace rtoken.tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public void Register_WhenUserAlreadyExists_ThrowsAppException()
        {
            var dataMock = new Mock<DataContext>();


        }
    }
}