namespace TeaTime.Common.Features.Runs.Queries;

using Abstractions;
using Models.Data;

public class GetRunQuery : IQuery<Run?>
{
    public long RunId { get; }

    public GetRunQuery(long runId)
    {
        RunId = runId;
    }
}
