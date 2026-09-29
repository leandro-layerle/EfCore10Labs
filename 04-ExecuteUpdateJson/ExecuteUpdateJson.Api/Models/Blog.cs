namespace ExecuteUpdateJson.Api.Models;

public sealed class Blog
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required BlogDetails Details { get; set; }
}
