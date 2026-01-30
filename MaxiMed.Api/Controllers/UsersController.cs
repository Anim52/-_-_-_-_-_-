using MaxiMed.Application.Users;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _svc;
    private readonly IUserAdminService _admin;

    public UsersController(IUserService svc, IUserAdminService admin)
    {
        _svc = svc;
        _admin = admin;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> Search([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, ct));

    [HttpGet("admin/search")]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> AdminSearch([FromQuery] string? q, CancellationToken ct)
        => Ok(await _admin.SearchAsync(q, ct));

    [HttpGet("admin/{id:int}")]
    public async Task<ActionResult<UserEditDto?>> GetAdmin(int id, CancellationToken ct)
        => Ok(await _admin.GetAsync(id, ct));

    public sealed record UserUpsertRequest(UserEditDto Dto, string? NewPassword);

    [HttpPost("admin")]
    public async Task<ActionResult<int>> Create([FromBody] UserUpsertRequest req, CancellationToken ct)
        => Ok(await _admin.CreateAsync(req.Dto, req.NewPassword, ct));

    [HttpPut("admin")]
    public async Task<IActionResult> Update([FromBody] UserUpsertRequest req, CancellationToken ct)
    {
        await _admin.UpdateAsync(req.Dto, req.NewPassword, ct);
        return NoContent();
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> Roles(CancellationToken ct)
        => Ok(await _admin.GetRolesAsync(ct));

    [HttpGet("role-names")]
    public async Task<ActionResult<IReadOnlyList<string>>> RoleNames(CancellationToken ct)
        => Ok(await _admin.GetAllRoleNamesAsync(ct));
}
