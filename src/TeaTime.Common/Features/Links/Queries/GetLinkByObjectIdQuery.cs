namespace TeaTime.Common.Features.Links.Queries;

using Abstractions;
using Models;

public record GetObjectIdByLinkValueQuery(LinkType LinkType, string Value) : IQuery<long?>;
