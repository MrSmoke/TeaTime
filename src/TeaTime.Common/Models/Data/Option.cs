namespace TeaTime.Common.Models.Data;

public record Option : BaseNamedDataObject
{
    public required bool Available { get; init; }
    public required long GroupId { get; init; }
    public required long CreatedBy { get; init; }
}
