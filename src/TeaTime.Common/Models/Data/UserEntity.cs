namespace TeaTime.Common.Models.Data;

public record UserEntity : BaseEntity
{
    public required string DisplayName { get; init; }
    public required string Username { get; init; }
}
