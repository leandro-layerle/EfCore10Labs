using ComplexTypes.Api.Data;
using ComplexTypes.Api.Models.Owned;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComplexTypes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OwnedCustomersController : ControllerBase
{
    private readonly AppDbContext _context;

    public OwnedCustomersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("shared-address-owned")]
    public async Task<IActionResult> CreateWithSharedOwnedAddress()
    {
        var address =
            new OwnedAddress
            {
                Street = "Av. Libertador 5000",
                City = "Buenos Aires",
                PostalCode = "C1426",
                Country = "Argentina"
            };

        var customer =
            new OwnedCustomer
            {
                Name = "Fabrikam Owned",

                ShippingAddress = address,

                BillingAddress = address
            };

        try
        {
            _context.OwnedCustomers.Add(customer);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                saved = true,

                sameInstance =
                    ReferenceEquals(
                        customer.ShippingAddress,
                        customer.BillingAddress)
            });
        }
        catch (InvalidOperationException exception)
        {
            return Ok(new
            {
                saved = false,

                sameInstance =
                    ReferenceEquals(
                        customer.ShippingAddress,
                        customer.BillingAddress),

                exception =
                    exception.GetType().Name,

                message =
                    exception.Message
            });
        }
    }

    [HttpPost("same-value-addresses")]
    public async Task<IActionResult> CreateWithSameValueAddresses()
    {
        var shippingAddress =
            new OwnedAddress
            {
                Street = "Av. Libertador 5000",
                City = "Buenos Aires",
                PostalCode = "C1426",
                Country = "Argentina"
            };

        var billingAddress =
            new OwnedAddress
            {
                Street = "Av. Libertador 5000",
                City = "Buenos Aires",
                PostalCode = "C1426",
                Country = "Argentina"
            };

        var customer =
            new OwnedCustomer
            {
                Name = "Contoso Owned",

                ShippingAddress =
                    shippingAddress,

                BillingAddress =
                    billingAddress
            };

        _context.OwnedCustomers.Add(customer);

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
        try
        {
            var query =
                _context.OwnedCustomers
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
                translated = true,
                sql,
                customers
            });
        }
        catch (InvalidOperationException exception)
        {
            return Ok(new
            {
                translated = false,

                exception =
                    exception.GetType().Name,

                message =
                    exception.Message
            });
        }
    }
}