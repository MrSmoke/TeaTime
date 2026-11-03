namespace TeaTime.Common.Models.Data;

public record Option : BaseNamedDataObject
{
    public required long GroupId { get; init; }
    public required long CreatedBy { get; init; }
    public bool Available { get; init; } = true;
}
