using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParameterizedCollections.Api.Data;
using ParameterizedCollections.Api.Models;

namespace ParameterizedCollections.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Products.AnyAsync())
        {
            return Ok(new
            {
                message = "Los datos de prueba ya existen."
            });
        }

        await InsertSeedData();

        return Ok(new
        {
            message = "Productos de prueba creados."
        });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _context.Products.ExecuteDeleteAsync();

        await _context.Database.ExecuteSqlRawAsync(
            "DBCC CHECKIDENT ('Products', RESEED, 0);");

        await InsertSeedData();

        return Ok(new
        {
            message = "Datos reiniciados. Products vuelve a comenzar en Id 1."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products =
            await _context.Products
                .AsNoTracking()
                .OrderBy(product => product.Id)
                .ToListAsync();

        return Ok(products);
    }

    [HttpGet("default")]
    public async Task<IActionResult> Default(
        [FromQuery] int[] ids)
    {
        var products =
            await _context.Products
                .AsNoTracking()
                .Where(product => ids.Contains(product.Id))
                .OrderBy(product => product.Id)
                .ToListAsync();

        return Ok(products);
    }

    [HttpGet("constant")]
    public async Task<IActionResult> Constant(
        [FromQuery] int[] ids)
    {
        var products =
            await _context.Products
                .AsNoTracking()
                .Where(
                    product =>
                        EF.Constant(ids)
                            .Contains(product.Id))
                .OrderBy(product => product.Id)
                .ToListAsync();

        return Ok(products);
    }

    [HttpGet("parameter")]
    public async Task<IActionResult> Parameter(
        [FromQuery] int[] ids)
    {
        var products =
            await _context.Products
                .AsNoTracking()
                .Where(
                    product =>
                        EF.Parameter(ids)
                            .Contains(product.Id))
                .OrderBy(product => product.Id)
                .ToListAsync();

        return Ok(products);
    }

    [HttpGet("multiple-parameters")]
    public async Task<IActionResult> MultipleParameters(
        [FromQuery] int[] ids)
    {
        var products =
            await _context.Products
                .AsNoTracking()
                .Where(
                    product =>
                        EF.MultipleParameters(ids)
                            .Contains(product.Id))
                .OrderBy(product => product.Id)
                .ToListAsync();

        return Ok(products);
    }

    private async Task InsertSeedData()
    {
        _context.Products.AddRange(
            new Product { Name = "Notebook", Price = 1500m },
            new Product { Name = "Monitor", Price = 450m },
            new Product { Name = "Teclado", Price = 120m },
            new Product { Name = "Mouse", Price = 80m },
            new Product { Name = "Dock", Price = 220m },
            new Product { Name = "Webcam", Price = 160m },
            new Product { Name = "Headset", Price = 140m },
            new Product { Name = "SSD", Price = 180m },
            new Product { Name = "Memoria RAM", Price = 130m },
            new Product { Name = "Router", Price = 210m },
            new Product { Name = "Switch", Price = 190m },
            new Product { Name = "UPS", Price = 300m });

        await _context.SaveChangesAsync();
    }
}
