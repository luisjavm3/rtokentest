using rtoken.api.Data;
using Xunit;
using Moq;
using rtoken.api.Models.Entities;
using rtoken.tests.Helpers;
using rtoken.tests.Fixtures;
using rtoken.api.Services.AuthService;
using rtoken.api.Models.TokensManager;
using rtoken.api.Models;
using Microsoft.AspNetCore.Http;
using FluentAssertions;

namespace rtoken.tests.Services
{
    public class AuthServiceTests
    {
        [Fact]
        public async void RefreshToken_ThrowsAppException_WhenNotFindingRefreshToken()
        {
            // Arrange
            var dataContext = new Mock<DataContext>();
            var aTokenManage = new Mock<IAccessTokenManager>();
            var rTokenManager = new Mock<IRefreshTokenManager>();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            dataContext
                .Setup(x => x.RefreshTokens)
                .Returns(TestFunctions.GetDbSet<RefreshToken>(TestData.RefreshTokens).Object);

            var sut = new AuthService
                        (
                            dataContext.Object,
                            aTokenManage.Object,
                            rTokenManager.Object,
                            httpContextAccessor.Object
                        );

            // Assert
            await sut
                .Awaiting(x => x.RefreshToken("refresh-token-value-6"))
                .Should()
                .ThrowAsync<AppException>()
                .WithMessage("Invalid token.");
        }
    }
}