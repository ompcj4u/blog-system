using BlogSystem.IntegrationTest.Common;
using BlogSystem.IntegrationTest.Common.Builder;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.IntegrationTest.Repositories;
public class UserRepositoryTests
{
    private readonly SqliteTestFixture _fixture;
    private readonly string ValidEmail = "mohammad@gmail.com";
    public UserRepositoryTests()
    {
        _fixture = new SqliteTestFixture();
    }
    [Fact]
    public async Task test()
    {
        // arrange
        using var context = _fixture.CreateContext();
        var userRepository = new UserRepository(context);
        var user = new UserBuilder().WithEmail(ValidEmail).Build();
        context.Users.Add(user);    
        await context.SaveChangesAsync();

        // act
        var result = await userRepository.GetUserByEmailAsync(ValidEmail);

        // assertion

        result.Should().NotBeNull();
        result.Email.Should().Be(new Email(ValidEmail));
        result.Id.Should().Be(user.Id);
    }
}
