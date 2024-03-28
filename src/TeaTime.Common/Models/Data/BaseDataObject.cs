namespace TeaTime.Common.Models.Data;

using System;

public abstract record BaseDataObject
{
    public required long Id { get; init; }

    /// <summary>
    /// The date the object was created
    /// </summary>
    public required DateTimeOffset CreatedDate { get; init; }
}
