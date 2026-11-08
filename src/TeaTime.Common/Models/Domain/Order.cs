namespace TeaTime.Common.Models.Domain;

using System;

public record Order(long Id, Option? Option, User? User, DateTimeOffset CreatedDate);
