namespace Platform.Contracts;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string username);
    string GenerateRefreshToken();
}
