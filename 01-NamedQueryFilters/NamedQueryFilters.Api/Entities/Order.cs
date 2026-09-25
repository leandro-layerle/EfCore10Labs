namespace NamedQueryFilters.Api.Entities;

public class Order
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public int TenantId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
}