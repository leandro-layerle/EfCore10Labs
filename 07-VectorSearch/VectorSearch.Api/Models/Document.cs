using Microsoft.Data.SqlTypes;

namespace VectorSearch.Api.Models;

public sealed class Document
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string Content { get; set; }

    public SqlVector<float> Embedding { get; set; }
}
