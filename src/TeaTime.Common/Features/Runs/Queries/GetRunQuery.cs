namespace TeaTime.Common.Features.Runs.Queries;

using Abstractions;
using Models.Domain;

public record GetRunQuery(long RunId) : IQuery<Run?>;
