using System.Security.Claims;

namespace OrdersInfrastructure.Security;

public interface IJwtTokenProvider
{
    string GenerateToken(int clientId, string user, Guid requestId);
    ClaimsPrincipal ValidateToken(string token);
}