using System;
using System.Collections.Generic;
using System.Linq;
using rtoken.api.Models.Entities;

namespace rtoken.tests.Fixtures
{

    public static class TestData
    {

        // Test data for the DbSet<User> getter
        public static IQueryable<RefreshToken> RefreshTokens
        {
            get
            {
                return new List<RefreshToken>
                {
                    // ↓↓↓ Expired but not revoked.
                    new RefreshToken{
                        Id=1,
                        Value="refresh-token-value-1",
                        ExpiresAt=DateTime.UtcNow.AddMinutes(-1),
                        RevokedAt = null,
                    },
                    // ↓↓↓ Expired AND revoked.
                    new RefreshToken{
                        Id=2,
                        Value="refresh-token-value-2",
                        ExpiresAt=DateTime.UtcNow.AddMinutes(-1),
                        RevokedAt=DateTime.UtcNow.AddMinutes(-1)
                    },
                    new RefreshToken{
                        Id=3,
                        Value="refresh-token-value-3",
                    },
                    new RefreshToken{
                        Id=4,
                        Value="refresh-token-value-4",
                    },
                    new RefreshToken{
                        Id=5,
                        Value="refresh-token-value-5",
                    },
                }
                .AsQueryable();
            }
        }
    }

}