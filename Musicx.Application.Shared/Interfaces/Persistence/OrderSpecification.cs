using Musicx.Application.Shared.Enums;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Shared.Interfaces.Persistence;

public abstract record OrderSpecification<T> where T : BaseInputModel
{
    public short? CreatedAt { get; init; }
    public short? UpdatedAt { get; init; }

    public abstract void Validate();

    public IEnumerable<OrderClause> ToClauses()
    {
        return GetType().GetProperties()
            .Select(p => (Prop: p, Pos: (short?)p.GetValue(this)))
            .Where(x => x.Pos is not null)
            .OrderBy(x => Math.Abs(x.Pos!.Value))
            .Select(x => ToClause(x.Prop.Name, x.Pos!.Value));
    }
    
    protected abstract OrderClause ToClause(string propName, short pos);
}