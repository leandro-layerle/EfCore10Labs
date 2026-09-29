using DynamicExecuteUpdate.Api.Data;
using DynamicExecuteUpdate.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicExecuteUpdate.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(
        AppDbContext context)
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

        _context.Products.AddRange(
            CreateSeedProducts());

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Productos creados."
        });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        // Eliminamos todos los productos.
        await _context.Products
            .ExecuteDeleteAsync();

        // IMPORTANTE:
        // ExecuteDelete elimina las filas, pero NO reinicia el IDENTITY.
        // Lo reiniciamos para que el próximo producto vuelva a tener Id = 1.
        await _context.Database.ExecuteSqlRawAsync(
            "DBCC CHECKIDENT ('Products', RESEED, 0);");

        // Volvemos a cargar exactamente los mismos datos iniciales.
        _context.Products.AddRange(
            CreateSeedProducts());

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Datos reiniciados. Los IDs vuelven a comenzar en 1."
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<Product>>> Get()
    {
        var products =
            await _context.Products
                .AsNoTracking()
                .OrderBy(product => product.Id)
                .ToListAsync();

        return Ok(products);
    }

    [HttpPatch("{id:int}/fixed")]
    public async Task<IActionResult> FixedUpdate(
        int id,
        [FromBody] UpdateProductRequest request)
    {
        if (request.Price is null)
        {
            return BadRequest(new
            {
                message =
                    "Para esta prueba fija se requiere Price."
            });
        }

        var affectedRows =
            await _context.Products
                .Where(product =>
                    product.Id == id)
                .ExecuteUpdateAsync(
                    setters =>
                        setters.SetProperty(
                            product => product.Price,
                            request.Price.Value));

        if (affectedRows == 0)
        {
            return NotFound();
        }

        return Ok(new
        {
            affectedRows
        });
    }

    [HttpPatch("{id:int}/dynamic")]
    public async Task<IActionResult> DynamicUpdate(
        int id,
        [FromBody] UpdateProductRequest request)
    {
        if (request.Name is null &&
            request.Price is null &&
            request.Stock is null &&
            request.IsActive is null)
        {
            return BadRequest(new
            {
                message =
                    "Debe indicar al menos una propiedad para actualizar."
            });
        }

        var affectedRows =
            await _context.Products
                .Where(product =>
                    product.Id == id)
                .ExecuteUpdateAsync(setters =>
                {
                    if (request.Name is not null)
                    {
                        setters.SetProperty(
                            product => product.Name,
                            request.Name);
                    }

                    if (request.Price is not null)
                    {
                        setters.SetProperty(
                            product => product.Price,
                            request.Price.Value);
                    }

                    if (request.Stock is not null)
                    {
                        setters.SetProperty(
                            product => product.Stock,
                            request.Stock.Value);
                    }

                    if (request.IsActive is not null)
                    {
                        setters.SetProperty(
                            product => product.IsActive,
                            request.IsActive.Value);
                    }
                });

        if (affectedRows == 0)
        {
            return NotFound();
        }

        return Ok(new
        {
            affectedRows
        });
    }

    private static Product[] CreateSeedProducts()
    {
        return
        [
            new Product
            {
                Name = "Mechanical Keyboard",
                Price = 120m,
                Stock = 15,
                IsActive = true
            },

            new Product
            {
                Name = "USB-C Dock",
                Price = 180m,
                Stock = 8,
                IsActive = true
            },

            new Product
            {
                Name = "27-inch Monitor",
                Price = 350m,
                Stock = 5,
                IsActive = true
            }
        ];
    }
}
