using LeftRightJoin.Api.Data;
using LeftRightJoin.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeftRightJoin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class JoinsController : ControllerBase
{
    private readonly AppDbContext _context;

    public JoinsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Customers.AnyAsync() ||
            await _context.Orders.AnyAsync())
        {
            return Ok(new
            {
                message = "Los datos de prueba ya existen."
            });
        }

        await InsertSeedData();

        return Ok(new
        {
            message = "Datos de prueba creados."
        });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await _context.Orders.ExecuteDeleteAsync();
        await _context.Customers.ExecuteDeleteAsync();

        // IMPORTANTE:
        // ExecuteDelete no reinicia los IDENTITY de SQL Server.
        // Los reiniciamos para que TODO el archivo .http sea reproducible.
        await _context.Database.ExecuteSqlRawAsync(
            "DBCC CHECKIDENT ('Orders', RESEED, 0);");

        await _context.Database.ExecuteSqlRawAsync(
            "DBCC CHECKIDENT ('Customers', RESEED, 0);");

        await InsertSeedData();

        return Ok(new
        {
            message = "Datos reiniciados. Customers y Orders vuelven a comenzar en Id 1."
        });
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetData()
    {
        var customers =
            await _context.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.Id)
                .ToListAsync();

        var orders =
            await _context.Orders
                .AsNoTracking()
                .OrderBy(order => order.Id)
                .ToListAsync();

        return Ok(new
        {
            customers,
            orders
        });
    }

    [HttpGet("left/legacy")]
    public async Task<IActionResult> LegacyLeftJoin()
    {
        // ANTES DE .NET 10:
        // LEFT JOIN requería reconocer este patrón exacto:
        // GroupJoin + SelectMany + DefaultIfEmpty.
        var result =
            await _context.Customers
                .GroupJoin(
                    _context.Orders,
                    customer => customer.Id,
                    order => order.CustomerId,
                    (customer, orders) => new
                    {
                        Customer = customer,
                        Orders = orders
                    })
                .SelectMany(
                    item => item.Orders.DefaultIfEmpty(),
                    (item, order) => new
                    {
                        CustomerId = item.Customer.Id,
                        CustomerName = item.Customer.Name,
                        OrderId = order == null
                            ? (int?)null
                            : order.Id,
                        Total = order == null
                            ? (decimal?)null
                            : order.Total
                    })
                .OrderBy(item => item.CustomerId)
                .ThenBy(item => item.OrderId)
                .ToListAsync();

        return Ok(result);
    }

    [HttpGet("left")]
    public async Task<IActionResult> LeftJoin()
    {
        // .NET 10 agrega LeftJoin como operador LINQ.
        // EF Core 10 reconoce el operador y lo traduce a LEFT JOIN.
        var result =
            await _context.Customers
                .LeftJoin(
                    _context.Orders,
                    customer => customer.Id,
                    order => order.CustomerId,
                    (customer, order) => new
                    {
                        CustomerId = customer.Id,
                        CustomerName = customer.Name,
                        OrderId = order == null
                            ? (int?)null
                            : order.Id,
                        Total = order == null
                            ? (decimal?)null
                            : order.Total
                    })
                .OrderBy(item => item.CustomerId)
                .ThenBy(item => item.OrderId)
                .ToListAsync();

        return Ok(result);
    }

    [HttpGet("right")]
    public async Task<IActionResult> RightJoin()
    {
        // RightJoin conserva TODAS las filas de la segunda secuencia.
        // EF Core 10 lo traduce a RIGHT JOIN en SQL Server.
        var result =
            await _context.Customers
                .RightJoin(
                    _context.Orders,
                    customer => customer.Id,
                    order => order.CustomerId,
                    (customer, order) => new
                    {
                        CustomerId = customer == null
                            ? (int?)null
                            : customer.Id,
                        CustomerName = customer == null
                            ? "[SIN CLIENTE]"
                            : customer.Name,
                        OrderId = order.Id,
                        Total = order.Total
                    })
                .OrderBy(item => item.OrderId)
                .ToListAsync();

        return Ok(result);
    }

    private async Task InsertSeedData()
    {
        _context.Customers.AddRange(
            new Customer
            {
                Name = "Ana"
            },
            new Customer
            {
                Name = "Bruno"
            },
            new Customer
            {
                Name = "Carla"
            });

        await _context.SaveChangesAsync();

        _context.Orders.AddRange(
            new Order
            {
                CustomerId = 1,
                Total = 120m
            },
            new Order
            {
                CustomerId = 1,
                Total = 85m
            },
            new Order
            {
                CustomerId = 2,
                Total = 200m
            },
            new Order
            {
                // No existe Customer 99.
                // Esta fila permite demostrar qué preserva RightJoin.
                CustomerId = 99,
                Total = 50m
            });

        await _context.SaveChangesAsync();
    }
}
