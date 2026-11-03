namespace TeaTime.Common.Features.Statistics.Models;

public record GlobalTotals
{
    public required long OrdersMade { get; init; }
    public required long RunsEnded { get; init; }
}
