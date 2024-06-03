namespace TeaTime.Common.Models.Data;

public record IllMake : BaseDataObject
{
    public required long RunId { get; init; }
    public required long UserId { get; init; }
}
