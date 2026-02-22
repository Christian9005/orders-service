using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersApplication.DTOs;
using OrdersInfrastructure.Security;

namespace OrdersApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenProvider jwtTokenProvider;

    public AuthController(IJwtTokenProvider jwtTokenProvider)
    {
        this.jwtTokenProvider = jwtTokenProvider;
    }

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenRequest request)
    {
        var requestId = Guid.NewGuid();
        var token = jwtTokenProvider.GenerateToken(
            request.ClientId,
            request.User,
            requestId);

        return Ok(new
        {
            token,
            requestId,
            expiresIn = 3600
        });
    }
}
