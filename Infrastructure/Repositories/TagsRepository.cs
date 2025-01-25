using Application.Abstraction;
using Application.Dtos.Enums;
using Application.Dtos;
using Domain.Aggregates;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal class TagsRepository : ITagsRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public TagsRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<bool> AnyExistsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Tags.AnyAsync();
        }

        public async Task RemoveAllAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Tags").ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task SaveAsync(TagAggregate aggregate)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.Tags.AddRangeAsync(aggregate.Tags).ConfigureAwait(false);

            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<List<Tag>> GetTagsAsync(TagFilter filter)
        {
            await using var context = _contextFactory.CreateDbContext();
            var query = context.Tags.AsQueryable();

            query = filter.SortBy switch
            {
                SortBy.Name => filter.SortOrder == SortOrder.Desc
                    ? query.OrderByDescending(t => t.Name)
                    : query.OrderBy(t => t.Name),
                SortBy.Percentage => filter.SortOrder == SortOrder.Desc
                    ? query.OrderByDescending(t => t.Count)
                    : query.OrderBy(t => t.Count),
                _ => throw new ArgumentException("Invalid sort option", nameof(filter.SortBy))
            };

            return await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<int> GetTotalCountAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Tags.CountAsync();
        }
    }
}
