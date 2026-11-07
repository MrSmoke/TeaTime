namespace TeaTime.Common.Models.Data;

public record OrderEntity : BaseEntity
{
    public required long RunId { get; init; }
    public required long UserId { get; init; }
    public required long OptionId { get; init; }
}
