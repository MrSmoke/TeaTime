namespace TeaTime.Common.Features.Runs.Queries;

using Abstractions;
using Models.Data;

public record GetRunQuery(long RunId) : IQuery<Run?>;
