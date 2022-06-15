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
using System.Linq;
using System;
using System.Collections;

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

        [Fact]
        public async void RefreshToken_ThorwsAppException_WhenFindingExpiredButNotRevokedRefreshToken()
        {
            // Arrange
            var dataContext = new Mock<DataContext>();
            var aTokenManage = new Mock<IAccessTokenManager>();
            var rTokenManager = new Mock<IRefreshTokenManager>();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();

            var rTokenDbSet = TestFunctions.GetDbSet<RefreshToken>(TestData.RefreshTokens).Object;

            dataContext
                .Setup(x => x.RefreshTokens)
                .Returns(rTokenDbSet);

            httpContextAccessor
                .Setup(x => x.HttpContext.Request.Headers.ContainsKey(It.IsAny<string>()))
                .Returns(true);

            var sut = new AuthService
                        (
                            dataContext.Object,
                            aTokenManage.Object,
                            rTokenManager.Object,
                            httpContextAccessor.Object
                        );

            await sut
                .Awaiting(x => x.RefreshToken("refresh-token-value-1"))
                .Should()
                .ThrowAsync<AppException>()
                .WithMessage("Token expired.");

            await sut
                .Awaiting(x => x.RefreshToken("refresh-token-value-2"))
                .Should()
                .NotThrowAsync<AppException>();

            await sut
                .Awaiting(x => x.RefreshToken("refresh-token-value-3"))
                .Should()
                .NotThrowAsync<AppException>();
        }

        // [Fact]
        // public async void RefreshToken_ThorwsAppException_WhenFindingRevokedRefreshToken()
        // {
        //     // Arrange
        //     var dataContext = new Mock<DataContext>();
        //     var aTokenManage = new Mock<IAccessTokenManager>();
        //     var rTokenManager = new Mock<IRefreshTokenManager>();
        //     var httpContextAccessor = new Mock<IHttpContextAccessor>();

        //     var rTokenDbSet = TestFunctions.GetDbSet<RefreshToken>(TestData.RefreshTokens);

        //     dataContext
        //         .Setup(x => x.RefreshTokens)
        //         .Returns(rTokenDbSet.Object);
        //     // dataContext
        //     //     .Setup(x => x.RefreshTokens.Where(It.IsAny<Func<object, bool>>()))
        //     //     // .Returns(rTokenDbSet.Object);
        //     //     .Verifiable();

        //     httpContextAccessor
        //         .Setup(x => x.HttpContext.Request.Headers.ContainsKey(It.IsAny<string>()))
        //         .Returns(true);

        //     var sut = new AuthService
        //                 (
        //                     dataContext.Object,
        //                     aTokenManage.Object,
        //                     rTokenManager.Object,
        //                     httpContextAccessor.Object
        //                 );

        //     await sut
        //         .Awaiting(x => x.RefreshToken("refresh-token-value-2"))
        //         .Should()
        //         .ThrowAsync<AppException>()
        //         .WithMessage("Token revoked.");
        // }
    }
}