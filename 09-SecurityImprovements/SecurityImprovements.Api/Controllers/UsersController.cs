using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SecurityImprovements.Api.Data;
using SecurityImprovements.Api.Models;

namespace SecurityImprovements.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Users.AnyAsync())
        {
            return Ok(new { message = "Los datos de prueba ya existen." });
        }

        await InsertSeedData();
        return Ok(new { message = "Usuarios de prueba creados." });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _context.Users.ExecuteDeleteAsync();
        await _context.Database.ExecuteSqlRawAsync(
            "DBCC CHECKIDENT ('Users', RESEED, 0);");
        await InsertSeedData();

        return Ok(new
        {
            message = "Datos reiniciados. Users vuelve a comenzar en Id 1."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users =
            await _context.Users
                .AsNoTracking()
                .OrderBy(user => user.Id)
                .ToListAsync();

        return Ok(users);
    }

    [HttpGet("roles-inline")]
    public async Task<IActionResult> GetByRolesInline(
        [FromQuery] string[] roles)
    {
        var users =
            await _context.Users
                .AsNoTracking()
                .Where(
                    user =>
                        EF.Constant(roles)
                            .Contains(user.Role))
                .OrderBy(user => user.Id)
                .ToListAsync();

        return Ok(users);
    }

    [HttpGet("safe-email")]
    public async Task<IActionResult> GetByEmailSafe(
        [FromQuery] string email)
    {
        var users =
            await _context.Users
                .FromSql(
                    $"SELECT * FROM Users WHERE Email = {email}")
                .AsNoTracking()
                .ToListAsync();

        return Ok(users);
    }

    [HttpGet("raw-warning")]
    public async Task<IActionResult> RawWarning(
        [FromQuery] string fieldName)
    {
        // EF Core 10 agrega un analyzer que advierte cuando una concatenación
        // ocurre directamente dentro de una invocación de una API Raw SQL.
        // Este endpoint existe deliberadamente para hacer visible el warning.
        var users =
            await _context.Users
                .FromSqlRaw(
                    "SELECT * FROM Users WHERE [" +
                    fieldName +
                    "] IS NULL")
                .AsNoTracking()
                .ToListAsync();

        return Ok(users);
    }

    [HttpGet("raw-validated")]
    public async Task<IActionResult> RawValidated(
        [FromQuery] string fieldName,
        [FromQuery] string value)
    {
        var allowedFields =
            new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["Email"] = "Email",
                ["Role"] = "Role",
                ["DisplayName"] = "DisplayName"
            };

        if (!allowedFields.TryGetValue(
                fieldName,
                out var safeFieldName))
        {
            return BadRequest(new
            {
                message = "Campo no permitido.",
                allowedFields = allowedFields.Keys
            });
        }

        var sql =
            $"SELECT * FROM Users WHERE [{safeFieldName}] = @value";

        var parameter =
            new SqlParameter("value", value);

        var users =
            await _context.Users
                .FromSqlRaw(sql, parameter)
                .AsNoTracking()
                .ToListAsync();

        return Ok(users);
    }

    private async Task InsertSeedData()
    {
        _context.Users.AddRange(
            new User
            {
                DisplayName = "Ana Torres",
                Email = "ana@demo.local",
                Role = "Administrator"
            },
            new User
            {
                DisplayName = "Bruno Diaz",
                Email = "bruno@demo.local",
                Role = "Manager"
            },
            new User
            {
                DisplayName = "Carla Gomez",
                Email = "carla@demo.local",
                Role = "User"
            },
            new User
            {
                DisplayName = "Diego Lopez",
                Email = "diego@demo.local",
                Role = "Manager"
            },
            new User
            {
                DisplayName = "Elena Ruiz",
                Email = "elena@demo.local",
                Role = "User"
            });

        await _context.SaveChangesAsync();
    }
}
