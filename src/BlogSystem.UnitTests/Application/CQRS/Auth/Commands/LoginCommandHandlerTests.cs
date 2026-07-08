using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.CQRS.Auth.Commands;
using Application.Interfaces;
using BlogSystem.UnitTests.Helper;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace BlogSystem.UnitTests.Application.CQRS.Auth.Commands;
public class LoginCommandHandlerTests
{
    private const string ValidPassword = "123456";
    private readonly Mock<IUserRepository> userRepositoryMock;
    private readonly Mock<IJwtService> jwtServiceMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly LoginCommandHandler handler;
    public LoginCommandHandlerTests()
    {
        userRepositoryMock = new Mock<IUserRepository>();
        jwtServiceMock = new Mock<IJwtService>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        handler = new LoginCommandHandler(userRepositoryMock.Object, jwtServiceMock.Object, unitOfWorkMock.Object);
    }
    //LoginCommandHandler_Should_Return_Failure_When_User_Does_Not_Exist
    [Fact]
    public async Task Should_Return_Success_When_Email_And_Password_Are_Valid()
    {
        // arrange
        var accessToken = "access - token";
        var refreshToken = "refresh - Token";
        var user = UserMother.DefaultUser(ValidPassword);


        userRepositoryMock.Setup(s => s.GetUserByEmailAsync(user.Email.Value)) // Arrange only what matters
            .ReturnsAsync(user);
        jwtServiceMock.Setup(t => t.GenerateToken(user)).Returns(accessToken);
        jwtServiceMock.Setup(t => t.GenerateRefreshToken()).Returns(refreshToken);

        var command = new LoginCommand(user.Email.Value, ValidPassword);

        // act
        var result = await handler.Handle(command, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();
        userRepositoryMock.Verify(t => t.GetUserByEmailAsync(user.Email.Value), Times.Once());
        userRepositoryMock.Verify(t => t.UpdateAsync(user), Times.Once());
        // -------------------------
        // these are the same
        user.RefreshTokenExpiry.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
        user.RefreshToken.Should().Be(refreshToken); // state verification
        jwtServiceMock.Verify(t => t.GenerateToken(user), Times.Once()); // interaction verification
        // -------------------------
        unitOfWorkMock.Verify(t => t.SaveChangesAsync(CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task Should_Return_Failure_When_Password_Is_Invalid()
    {
        var user = UserMother.DefaultUser(ValidPassword);
        var command = new LoginCommand(user.Email.Value, "1234567"); // different password from 'user' object

        userRepositoryMock.Setup(t => t.GetUserByEmailAsync(user.Email.Value)).ReturnsAsync(user);

        var result = await handler.Handle(command, CancellationToken.None);


        result.IsSuccess.Should().BeFalse();
        jwtServiceMock.Verify(t => t.GenerateToken(user), Times.Never);

    }

    [Fact]
    public async Task Should_Return_Failure_When_User_Does_Not_Exist()
    {
        var user = (User?)null;
        var command = new LoginCommand("mohammad@gmail.com", ValidPassword);
        userRepositoryMock.Setup(t => t.GetUserByEmailAsync(command.Email)).ReturnsAsync(user);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        jwtServiceMock.Verify(t => t.GenerateToken(user), Times.Never);
        userRepositoryMock.Verify(t => t.UpdateAsync(user), Times.Never);
        unitOfWorkMock.Verify(t => t.SaveChangesAsync(CancellationToken.None), Times.Never);
    }

}
