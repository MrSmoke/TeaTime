namespace TeaTime.Common.Features.Options.Queries;

using Abstractions;
using Models.Data;

public record GetOptionQuery(long Id) : IQuery<Option?>;
