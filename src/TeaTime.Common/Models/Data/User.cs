namespace TeaTime.Common.Models.Data;

public record User : BaseDataObject
{
    public required string DisplayName { get; init; }
    public required string Username { get; init; }
}
