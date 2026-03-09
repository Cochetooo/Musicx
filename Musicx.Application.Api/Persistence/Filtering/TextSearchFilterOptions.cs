namespace Musicx.Application.API.Persistence.Filtering;

public sealed record TextSearchFilterOptions
{
    public bool Exact { get; init; }

    private double _similarity = 0.4;

    public double Similarity
    {
        get => _similarity;
        init => _similarity = value < 0 ? 0 : value;
    }
}