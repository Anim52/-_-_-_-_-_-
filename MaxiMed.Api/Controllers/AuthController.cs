using MaxiMed.Application.Auth;
using MaxiMed.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    public sealed record LoginRequest(string Login, string Password);

    [HttpPost("login")]
    public async Task<ActionResult<User?>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await _auth.LoginAsync(req.Login, req.Password);
        return Ok(user);
    }
}
