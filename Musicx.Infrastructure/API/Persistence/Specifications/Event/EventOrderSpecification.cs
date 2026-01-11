using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.Event;
using Musicx.Infrastructure.API.Persistence.Columns.Event;

namespace Musicx.Infrastructure.API.Persistence.Specifications.Event;

public sealed record EventOrderSpecification : OrderSpecification<InEvent>
{
    public short? Address { get; init; }
    public short? BeginDate { get; init; }
    public short? Country { get; init; }
    public short? EndDate { get; init; }
    public short? IsFestival { get; init; }
    public short? IsVisible { get; init; }
    public short? Name { get; init; }
    public short? Town { get; init; }
    public short? Venue { get; init; }
    public short? ZipCode { get; init; }

    public override void Validate()
    {
        
    }
    
    protected override OrderClause ToClause(string propName, short pos)
    {
        var dir = pos > 0 ? OrderDirection.Asc : OrderDirection.Desc;

        return propName switch
        {
            nameof(CreatedAt) => new($"ev0.{EventColumns.CreatedAt}", dir),
            nameof(UpdatedAt) => new($"ev0.{EventColumns.UpdatedAt}", dir),
            nameof(Address) => new($"ev0.{EventColumns.Address}", dir),
            nameof(BeginDate) => new($"ev0.{EventColumns.BeginDate}", dir),
            nameof(Country) => new($"ev0.{EventColumns.Country}", dir),
            nameof(EndDate) => new($"ev0.{EventColumns.EndDate}", dir),
            nameof(IsFestival) => new($"ev0.{EventColumns.IsFestival}", dir),
            nameof(IsVisible) => new($"ev0.{EventColumns.IsVisible}", dir),
            nameof(Name) => new($"ev0.{EventColumns.Name}", dir),
            nameof(Town) => new($"ev0.{EventColumns.Town}", dir),
            nameof(Venue) => new($"ev0.{EventColumns.Venue}", dir),
            nameof(ZipCode) => new($"ev0.{EventColumns.ZipCode}", dir),
            _ => throw new ArgumentOutOfRangeException(nameof(propName), propName, null)
        };
    }
}