using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Musicx.Application.Shared.Interfaces.Common;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Desktop.Interfaces.Persistence;
using Musicx.Application.Desktop.Specifications;
using Musicx.Domain.Models;

namespace Musicx.Infrastructure.Persistence.Repositories;

internal sealed class LabelRepository(
    AppDbContext context,
    ILabelCache labelCache,
    ILoggerFactory loggerFactory) : ILabelRepository
{
    private readonly ILogger<LabelRepository> _logger = loggerFactory.CreateLogger<LabelRepository>();
    
    public async Task DeleteAsync(long id)
    {
        _logger.Db($"📄 Delete Label : {id}");
        
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
            _logger.Fatal($"❌ Could not delete id {id}", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Label?> FindByIdAsync(long id, IQuerySpecification<Label>? labelQuerySpecification = null)
    {
        _logger.Db($"📄 Find By Id Label : {id}");
        
        var labelSet = context.Labels;
        GetIncludes(labelSet, labelQuerySpecification);
        
        return await labelSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Label>> FindAsync(int skip = 0, int take = 100, Expression<Func<Label, bool>>? filter = null,
        IQuerySpecification<Label>? labelQuerySpecification = null)
    {
        _logger.Db($"📄 Find Label");
        
        var labelSet = context.Labels;
        
        GetIncludes(labelSet, labelQuerySpecification);
        var query = labelSet.AsQueryable();

        if (null != filter)
        {
            query = query.Where(filter);
        }
        
        return await query
            .AsNoTracking()
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<Label>> FindIn(IEnumerable<long> ids, IQuerySpecification<Label>? labelQuerySpecification = null)
    {
        _logger.Db("📄 Find In Label");

        var enumerable = ids as long[] ?? ids.ToArray();
        
        if (0 == enumerable.Length)
        {
            _logger.Debug("ℹ️ Empty List in Find In");
            return [];
        }
        
        var labelSet = context.Labels;
        GetIncludes(labelSet, labelQuerySpecification);
        
        return await labelSet
            .Where(s => enumerable.Contains(s.Id))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountAsync()
    {
        return await context.Labels.CountAsync();
    }

    public async Task<long> SaveAsync(Label entity)
    {
        _logger.Db("📄 Save Label");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (0 == entity.Id)
            {
                context.Labels.Add(entity);
            }
            else
            {
                context.Labels.Update(entity);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return entity.Id;
        }
        catch (Exception ex)
        {
            _logger.Fatal("❌ Failed saving.", ex);
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<long>> SaveAllAsync(IEnumerable<Label> entities)
    {
        _logger.Db("📄 SaveAll Label");
        
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = new List<long>();

        try
        {
            foreach (var label in entities)
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

    private void GetIncludes(in DbSet<Label> labelSet, IQuerySpecification<Label>? querySpecification = null)
    {
        if (null == querySpecification)
        {
            return;
        }
        
        var labelQuerySpecification = (LabelQuerySpecification)querySpecification;

        if (labelQuerySpecification.IncludeReleases)
        {
            labelSet.Include(s => s.Releases);
        }
    }
}