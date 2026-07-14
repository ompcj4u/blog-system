using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories;
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await dbSet.SingleOrDefaultAsync(x => x.Email.Value == email);
    }

    public Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return dbSet.SingleOrDefaultAsync(t => t.RefreshToken == refreshToken
        && t.RefreshTokenExpiry > DateTime.UtcNow);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return !await dbSet.AnyAsync(t=>t.Email.Value == email);
    }
}
