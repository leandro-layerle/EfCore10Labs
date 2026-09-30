namespace SecurityImprovements.Api.Models;

public sealed class User
{
    public int Id { get; set; }

    public required string DisplayName { get; set; }

    public required string Email { get; set; }

    public required string Role { get; set; }
}
