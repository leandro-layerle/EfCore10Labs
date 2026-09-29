namespace LeftRightJoin.Api.Models;

public sealed class Order
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public decimal Total { get; set; }
}
