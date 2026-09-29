namespace ExecuteUpdateJson.Api.Models;

public sealed class BlogDetails
{
    public required string Title { get; set; }

    public required string Category { get; set; }

    public int Views { get; set; }

    public bool Featured { get; set; }
}
