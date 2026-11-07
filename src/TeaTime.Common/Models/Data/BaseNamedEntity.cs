namespace TeaTime.Common.Models.Data;

public abstract record BaseNamedEntity : BaseEntity
{
    /// <summary>
    /// The name of the object
    /// </summary>
    public required string Name { get; init; }
}
