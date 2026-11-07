namespace TeaTime.Common.Models.Domain;

using Data;

public record OrderModel : BaseEntity
{
    public required RunEntity Run { get; init; }
    public required UserEntity? User { get; init; }
    public required OptionEntity? Option { get; init; }
}
