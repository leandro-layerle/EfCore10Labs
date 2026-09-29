namespace ComplexTypes.Api.Models.Owned;

public sealed class OwnedAddress
{
    public required string Street { get; set; }

    public required string City { get; set; }

    public required string PostalCode { get; set; }

    public required string Country { get; set; }
}