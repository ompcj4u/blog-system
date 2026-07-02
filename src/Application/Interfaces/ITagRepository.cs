using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface ITagRepository : IGenericRepository<Tag>
{
    Task<List<Tag>> GetUnusedTagsAsync();
    Task<List<Tag>> GetTagsByIdsAsync(List<Guid> tagIds);
    Task<Tag?> GetTagByNameAsync(string name);
}
