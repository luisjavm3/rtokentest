using Xunit;
using System.Security.Cryptography;
using rtoken.api.Utils;
using FluentAssertions;

namespace rtoken.tests.Utils
{
    public class PasswordUtilsTests
    {
        [Fact]
        public void OnGetPasswordHashSuccess_ReturnsArrayOfBytes()
        {
            // Arrange
            var sut = PasswordUtils.GetPasswordHash;

            //Act
            sut("random_password", out var passwordHash, out var passwordSalt);

            //Assert
            passwordHash.Should().BeOfType<byte[]>();
            passwordSalt.Should().BeOfType<byte[]>();
        }

        [Theory]
        [InlineData("password1")]
        [InlineData("password2")]
        [InlineData("password3")]
        public void MatchHashes_ComparesEqualityOfPasswords(string password)
        {
            // Arrange
            var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            hmac.Dispose();
            var sut = PasswordUtils.MatchHashes;

            // Act
            bool actual1 = sut(password, hash, salt);
            bool actual2 = sut("wrong_password", hash, salt);

            // Assert
            actual1.Should().BeTrue();
            actual2.Should().BeFalse();
        }
    }
}