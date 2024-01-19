namespace TeaTime.Common.Features.Options;

using Abstractions;
using Models.Data;

public record GetOptionQuery(long Id) : IQuery<Option?>;
