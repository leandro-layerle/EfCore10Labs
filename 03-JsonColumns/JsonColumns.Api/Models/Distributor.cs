namespace JsonColumns.Api.Models;

public sealed class Distributor
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<Address> ShippingCenters { get; set; } = [];
}