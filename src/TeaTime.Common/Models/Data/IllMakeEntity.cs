namespace TeaTime.Common.Models.Data;

public record IllMakeEntity : BaseEntity
{
    public required long RunId { get; init; }
    public required long UserId { get; init; }
}
