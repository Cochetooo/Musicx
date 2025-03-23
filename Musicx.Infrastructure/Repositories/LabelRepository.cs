using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Data;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Repositories;

public interface ILabelRepository : IRepository<LabelEntity>;

public class LabelRepository(AppDbContext context, ILoggerFactory loggerFactory) : ILabelRepository
{
    private readonly ILogger<LabelRepository> _logger = loggerFactory.CreateLogger<LabelRepository>();

    public async Task<LabelEntity?> FindById(ulong id)
    {
        return await context.Labels.FindAsync(id);
    }

    public async Task<List<LabelEntity>> FindAll(int skip = 0, int count = 100,
        Expression<Func<LabelEntity, bool>>? filter = null)
    {
        var query = context.Labels.AsQueryable();

        if (null != filter)
        {
            query = query.Where(filter);
        }
        
        return await query
            .AsNoTracking()
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<LabelEntity>> FindIn(IList<ulong> ids)
    {
        if (ids.Count == 0)
        {
            return [];
        }
        
        return await context.Labels
            .Where(s => ids.Contains(s.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<uint> GetCount()
    {
        return (uint)await context.Labels.CountAsync();
    }

    public async Task<ulong> Save(LabelEntity label)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == label.Id)
            {
                context.Labels.Add(label);
            }
            else
            {
                context.Labels.Update(label);
            }
            
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return label.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ulong>> SaveAll(IList<LabelEntity> labels)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<ulong>();

        try
        {
            foreach (var label in labels)
            {
                if (0 == label.Id)
                {
                    context.Labels.Add(label);
                }
                else
                {
                    context.Labels.Update(label);
                }
                
                ids.Add(label.Id);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ids;
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Failed saving", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Delete(ulong id)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var label = await context.Labels.FindAsync(id);

            if (null != label)
            {
                context.Labels.Remove(label);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal($"❌ Failed deleting", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }
}