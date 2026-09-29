using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;
using VectorSearch.Api.Data;
using VectorSearch.Api.Models;

namespace VectorSearch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DocumentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DocumentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Documents.AnyAsync())
        {
            return Ok(new
            {
                message = "Los datos de prueba ya existen."
            });
        }

        await InsertSeedData();

        return Ok(new
        {
            message = "Datos vectoriales de prueba creados."
        });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _context.Documents.ExecuteDeleteAsync();

        await _context.Database.ExecuteSqlRawAsync(
            "DBCC CHECKIDENT ('Documents', RESEED, 0);");

        await InsertSeedData();

        return Ok(new
        {
            message = "Datos reiniciados. Documents vuelve a comenzar en Id 1."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents =
            await _context.Documents
                .AsNoTracking()
                .OrderBy(document => document.Id)
                .Select(document => new
                {
                    document.Id,
                    document.Title,
                    document.Content
                })
                .ToListAsync();

        return Ok(documents);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] float x = 1,
        [FromQuery] float y = 0,
        [FromQuery] float z = 0,
        [FromQuery] int take = 3)
    {
        if (take < 1 || take > 5)
        {
            return BadRequest(new
            {
                message = "take debe estar entre 1 y 5."
            });
        }

        var queryVector =
            new SqlVector<float>(
                new float[]
                {
                    x,
                    y,
                    z
                });

        var results =
            await _context.Documents
                .AsNoTracking()
                .Select(document => new
                {
                    document.Id,
                    document.Title,
                    document.Content,
                    Distance =
                        EF.Functions.VectorDistance(
                            "cosine",
                            document.Embedding,
                            queryVector)
                })
                .OrderBy(document => document.Distance)
                .Take(take)
                .ToListAsync();

        return Ok(new
        {
            queryVector = new[] { x, y, z },
            metric = "cosine",
            results
        });
    }

    private async Task InsertSeedData()
    {
        _context.Documents.AddRange(
            new Document
            {
                Title = "Guía de EF Core 10",
                Content = "Nuevas características de Entity Framework Core 10.",
                Embedding = Vector(1.00f, 0.00f, 0.00f)
            },
            new Document
            {
                Title = "Consultas LINQ",
                Content = "Ejemplos prácticos de consultas LINQ con .NET.",
                Embedding = Vector(0.95f, 0.05f, 0.00f)
            },
            new Document
            {
                Title = "SQL Server 2025",
                Content = "Novedades del motor de base de datos SQL Server.",
                Embedding = Vector(0.80f, 0.20f, 0.00f)
            },
            new Document
            {
                Title = "Recetas de cocina",
                Content = "Ideas simples para cocinar durante la semana.",
                Embedding = Vector(0.00f, 1.00f, 0.00f)
            },
            new Document
            {
                Title = "Guía de viajes",
                Content = "Consejos para organizar un viaje.",
                Embedding = Vector(0.00f, 0.00f, 1.00f)
            });

        await _context.SaveChangesAsync();
    }

    private static SqlVector<float> Vector(
        float x,
        float y,
        float z)
    {
        return new SqlVector<float>(
            new float[]
            {
                x,
                y,
                z
            });
    }
}
