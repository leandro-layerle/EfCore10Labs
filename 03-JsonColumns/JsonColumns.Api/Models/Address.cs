namespace JsonColumns.Api.Models;

public sealed class Address
{
    public required string Street { get; set; }

    public required string City { get; set; }

    public required string PostalCode { get; set; }

    public required string Country { get; set; }
}