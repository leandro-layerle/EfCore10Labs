using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigrationEf9To10.Api.Data;
using MigrationEf9To10.Api.Models;
namespace MigrationEf9To10.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProductsController(AppDbContext context) { _context = context; }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Products.AnyAsync()) return Ok(new { message = "Los datos ya existen." });
        await InsertSeedData();
        return Ok(new { message = "Productos creados." });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _context.Products.ExecuteDeleteAsync();
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0);");
        await InsertSeedData();
        return Ok(new { message = "Datos reiniciados desde Id 1." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        return Ok(products);
    }

    [HttpGet("by-ids")]
    public async Task<IActionResult> GetByIds([FromQuery] int[] ids)
    {
        var products = await _context.Products.AsNoTracking()
            .Where(product => ids.Contains(product.Id)).OrderBy(x => x.Id).ToListAsync();
        return Ok(products);
    }

    [HttpGet("by-category")]
    public async Task<IActionResult> GetByCategory([FromQuery] string category)
    {
        var products = await _context.Products.AsNoTracking()
            .Where(product => product.Category == category).OrderBy(x => x.Id).ToListAsync();
        return Ok(products);
    }

    private async Task InsertSeedData()
    {
        _context.Products.AddRange(
            new Product { Name="Notebook", Price=1500m, Category="Computación" },
            new Product { Name="Monitor", Price=450m, Category="Computación" },
            new Product { Name="Teclado", Price=120m, Category="Periféricos" },
            new Product { Name="Mouse", Price=80m, Category="Periféricos" },
            new Product { Name="Dock", Price=220m, Category="Accesorios" },
            new Product { Name="Webcam", Price=160m, Category="Periféricos" });
        await _context.SaveChangesAsync();
    }
}