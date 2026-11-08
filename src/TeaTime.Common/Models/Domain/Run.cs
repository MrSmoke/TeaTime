namespace TeaTime.Common.Models.Domain;

using System;
using System.Collections.Generic;

public record Run(long Id, IReadOnlyList<Order> Orders, bool Ended, DateTimeOffset StartDate, DateTimeOffset? EndDate);
