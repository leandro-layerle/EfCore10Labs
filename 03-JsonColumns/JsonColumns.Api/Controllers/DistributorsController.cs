using JsonColumns.Api.Data;
using JsonColumns.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JsonColumns.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DistributorsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DistributorsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Distributors.AnyAsync())
        {
            return Ok(new
            {
                message = "Los datos de prueba ya existen."
            });
        }

        var distributor =
            new Distributor
            {
                Name = "South Logistics",

                ShippingCenters =
                [
                    new Address
                    {
                        Street = "Av. del Libertador 5000",
                        City = "Buenos Aires",
                        PostalCode = "C1426",
                        Country = "Argentina"
                    },

                    new Address
                    {
                        Street = "Bv. San Juan 650",
                        City = "Córdoba",
                        PostalCode = "X5000",
                        Country = "Argentina"
                    }
                ]
            };

        _context.Distributors.Add(distributor);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Distributor creado."
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<Distributor>>> Get()
    {
        var distributors =
            await _context.Distributors
                .AsNoTracking()
                .OrderBy(distributor => distributor.Id)
                .ToListAsync();

        return Ok(distributors);
    }
}