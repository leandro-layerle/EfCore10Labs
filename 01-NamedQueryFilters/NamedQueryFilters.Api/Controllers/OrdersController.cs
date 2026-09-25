using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NamedQueryFilters.Api.Data;
using NamedQueryFilters.Api.Entities;

namespace NamedQueryFilters.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAll()
    {
        var orders = await _context.Orders
            .OrderBy(order => order.Id)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("include-deleted")]
    public async Task<ActionResult<List<Order>>> GetIncludingDeleted()
    {
        var orders = await _context.Orders
            .IgnoreQueryFilters()
            .OrderBy(order => order.Id)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("include-deleted-current-tenant")]
    public async Task<ActionResult<List<Order>>> GetIncludingDeletedForCurrentTenant()
    {
        var orders = await _context.Orders
            .IgnoreQueryFilters()
            .Where(order => order.TenantId == _context.CurrentTenantId)
            .OrderBy(order => order.Id)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("include-deleted-ef10")]
    public async Task<ActionResult<List<Order>>> GetIncludingDeletedEf10()
    {
        var orders = await _context.Orders
            .IgnoreQueryFilters([QueryFilterNames.SoftDeletion])
            .OrderBy(order => order.Id)
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("all-tenants-active")]
    public async Task<ActionResult<List<Order>>> GetActiveOrdersFromAllTenants()
    {
        var orders = await _context.Orders
            .IgnoreQueryFilters([QueryFilterNames.Tenant])
            .OrderBy(order => order.Id)
            .ToListAsync();

        return Ok(orders);
    }
}