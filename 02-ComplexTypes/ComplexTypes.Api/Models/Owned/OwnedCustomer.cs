namespace ComplexTypes.Api.Models.Owned;

public sealed class OwnedCustomer
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required OwnedAddress ShippingAddress { get; set; }

    public OwnedAddress? BillingAddress { get; set; }
}