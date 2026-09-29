using JsonColumns.Api.Data;
using JsonColumns.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JsonColumns.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed()
    {
        if (await _context.Customers.AnyAsync())
        {
            return Ok(new
            {
                message = "Los datos de prueba ya existen."
            });
        }

        var acme =
            new Customer
            {
                Name = "Acme Argentina",

                ShippingAddress = new Address
                {
                    Street = "Av. Corrientes 1234",
                    City = "Buenos Aires",
                    PostalCode = "C1043",
                    Country = "Argentina"
                },

                BillingAddress = new Address
                {
                    Street = "Av. Santa Fe 2500",
                    City = "Buenos Aires",
                    PostalCode = "C1123",
                    Country = "Argentina"
                }
            };

        var contoso =
            new Customer
            {
                Name = "Contoso Argentina",

                ShippingAddress = new Address
                {
                    Street = "San Martín 500",
                    City = "Córdoba",
                    PostalCode = "X5000",
                    Country = "Argentina"
                },

                BillingAddress = null
            };

        _context.Customers.AddRange(
            acme,
            contoso);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Customers creados."
        });
    }

    [HttpGet]
    public async Task<ActionResult<List<Customer>>> Get()
    {
        var customers =
            await _context.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.Id)
                .ToListAsync();

        return Ok(customers);
    }

    [HttpGet("shipping-city/{city}")]
    public async Task<IActionResult> GetByShippingCity(
    string city)
    {
        var query =
            _context.Customers
                .AsNoTracking()
                .Where(customer =>
                    customer.ShippingAddress.City == city)
                .OrderBy(customer => customer.Id);

        var sql =
            query.ToQueryString();

        var customers =
            await query.ToListAsync();

        return Ok(new
        {
            sql,
            customers
        });
    }

    [HttpGet("without-billing-address")]
    public async Task<IActionResult> GetWithoutBillingAddress()
    {
        var query =
            _context.Customers
                .AsNoTracking()
                .Where(customer =>
                    customer.BillingAddress == null)
                .OrderBy(customer => customer.Id);

        var sql =
            query.ToQueryString();

        var customers =
            await query.ToListAsync();

        return Ok(new
        {
            sql,
            customers
        });
    }

    [HttpGet("shipping-cities")]
    public async Task<IActionResult> GetShippingCities()
    {
        var query =
            _context.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.Id)
                .Select(customer => new
                {
                    customer.Name,
                    City =
                        customer.ShippingAddress.City
                });

        var sql =
            query.ToQueryString();

        var customers =
            await query.ToListAsync();

        return Ok(new
        {
            sql,
            customers
        });
    }
}