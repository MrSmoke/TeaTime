namespace TeaTime.Common.Models.Domain;

using Data;

public record OrderModel : BaseDataObject
{
    public required Run Run { get; init; }
    public required User? User { get; init; }
    public required Option? Option { get; init; }
}
