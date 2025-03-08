using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Data;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Repositories;

public class LabelRepository(AppDbContext context, ILoggerFactory loggerFactory) : IRepository<LabelEntity>
{
    private readonly ILogger _logger = loggerFactory.CreateLogger(typeof(LabelRepository));

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
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    public async Task Save(LabelEntity label)
    {
        await SaveAll([label]);
    }

    public async Task SaveAll(IList<LabelEntity> labels)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

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
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
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