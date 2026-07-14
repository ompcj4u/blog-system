using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.IntegrationTest.Common;
public class SqliteTestFixture : IDisposable
{

    private readonly SqliteConnection _connection;

    public SqliteTestFixture()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
    }

    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }


    public void Dispose()
    {
        _connection.Dispose();
    }
}
