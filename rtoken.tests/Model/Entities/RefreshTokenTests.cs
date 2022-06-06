using rtoken.api.Models.Entities;
using FluentAssertions;
using System;
using Xunit;

namespace rtoken.tests.Model
{
    public class RefreshTokenTests
    {
        [Fact]
        public void IsExpired_ReturnsFalse_WhenExpiresAtIsGreaterThanNow()
        {
            // Arrange
            var rToken = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            // Act
            bool actual = rToken.IsExpired;

            // Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void IsExpired_ReturnsTrue_WhenExpiresAtIsLessThanOrEqualsToNow()
        {
            // Arrange
            var rToken1 = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow
            };
            var rToken2 = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddHours(-1)
            };

            // Act
            bool actual1 = rToken1.IsExpired;
            bool actual2 = rToken2.IsExpired;

            // Assert
            actual1.Should().BeTrue();
            actual2.Should().BeTrue();
        }

        [Fact]
        public void IsRevoked_ReturnsFalse_WhenIsRevokedIsNull()
        {
            // Arrange
            var rToken = new RefreshToken
            {
                RevokedAt = null
            };

            // Act
            bool actual = rToken.IsRevoked;

            //Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void IsRevoked_ReturnsTrue_WhenIsRevokedIsNotNull()
        {
            // Arrange
            var rToken = new RefreshToken
            {
                RevokedAt = DateTime.Now
            };

            // Act
            bool actual = rToken.IsRevoked;

            //Assert
            actual.Should().BeTrue();
        }
    }
}