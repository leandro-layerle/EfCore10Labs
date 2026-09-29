namespace JsonColumns.Api.Models;

public sealed class Customer
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required Address ShippingAddress { get; set; }

    public Address? BillingAddress { get; set; }
}