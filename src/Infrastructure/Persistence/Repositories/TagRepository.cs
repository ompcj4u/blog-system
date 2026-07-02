using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories;
public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    public TagRepository(AppDbContext context) : base(context) { }

    public async Task<List<Tag>> GetUnusedTagsAsync()
    {
        return await dbSet
            .Where(t => !t.PostTags.Any())
            .ToListAsync();
    }

    public async Task<List<Tag>> GetTagsByIdsAsync(List<Guid> tagIds)
    {
        return await dbSet
            .Where(t => tagIds.Contains(t.Id))
            .ToListAsync();
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await dbSet
            .FirstOrDefaultAsync(t => t.TagName == name);
    }
}
