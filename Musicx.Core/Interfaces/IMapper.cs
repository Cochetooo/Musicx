namespace Musicx.Core.Interfaces;

public interface IMapper<TDto,TEntity> 
    where TDto : class
    where TEntity : class
{
    public TDto ToDto(TEntity entity);
    public TEntity ToEntity(TDto dto);
}