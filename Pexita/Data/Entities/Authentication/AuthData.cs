using Pexita.Data.Entities.Brands;
using Pexita.Data.Entities.User;

namespace Pexita.Data.Entities.Authentication
{
    public class AuthData
    {
    }
    public class JwtSettings
    {
        public string? SecretKey { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpiresInMinutes { get; set; }
    }
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public required int UserId { get; set; }
        public required UserModel User { get; set; }
    }

    public class UserRefreshTokenDTO
    {
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
    }

    public class BrandRefreshToken
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public bool IsExpired => DateTime.UtcNow >= Expires;

        public required int BrandID { get; set; }
        public required BrandModel Brand { get; set; }
    }
    public class BrandRefreshTokenDTO
    {
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
    }

}
