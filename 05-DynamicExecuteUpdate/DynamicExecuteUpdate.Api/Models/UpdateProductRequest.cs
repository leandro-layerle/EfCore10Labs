namespace DynamicExecuteUpdate.Api.Models;

public sealed class UpdateProductRequest
{
    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public bool? IsActive { get; set; }
}
