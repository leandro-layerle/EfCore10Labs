using ComplexTypes.Api.Data;
using ComplexTypes.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplexTypes.Api.Controllers;

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

        var customerWithBillingAddress =
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

        var customerWithoutBillingAddress =
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
            customerWithBillingAddress,
            customerWithoutBillingAddress);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Datos de prueba creados."
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

    [HttpPost("shared-address-complex")]
    public async Task<IActionResult> CreateWithSharedComplexAddress()
    {
        var address =
            new Address
            {
                Street = "Av. Libertador 5000",
                City = "Buenos Aires",
                PostalCode = "C1426",
                Country = "Argentina"
            };

        var customer =
            new Customer
            {
                Name = "Fabrikam Complex",

                ShippingAddress = address,

                BillingAddress = address
            };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            customer.Id,
            customer.Name,

            sameInstance =
                ReferenceEquals(
                    customer.ShippingAddress,
                    customer.BillingAddress),

            customer.ShippingAddress,
            customer.BillingAddress
        });
    }

    [HttpGet("same-shipping-and-billing-address")]
    public async Task<IActionResult> GetWithSameShippingAndBillingAddress()
    {
        var query =
            _context.Customers
                .AsNoTracking()
                .Where(customer =>
                    customer.BillingAddress != null &&
                    customer.ShippingAddress ==
                    customer.BillingAddress)
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
}