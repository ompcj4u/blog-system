using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

}
