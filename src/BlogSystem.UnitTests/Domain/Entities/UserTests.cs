using BlogSystem.UnitTests.Helper;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.UnitTests.Domain.Entities;
public class UserTests
{
    // happy path
    [Fact]
    public void ChangePassword_Should_UpdatePassword_When_Input_Is_Valid()
    {
        //assign
        var user = UserMother.DefaultUser("oldPassword");
        //act
        user.ChangePassword("oldPassword", "newPassword");
        // assert
        user.Password.Verify("newPassword").Should().BeTrue();
    }

    // sad path
    [Fact]
    public void User_ChangePassword_Should_Throw_For_Wrong_Old_Password()
    {
        //assign
        string newPassword = "newPassword";
        var user = UserMother.DefaultUser("oldPassword");
        //act
        var act = () => user.ChangePassword("veryOldPassword", newPassword);
        // assert
        act.Should().Throw<ArgumentException>();
        user.Password.Verify("oldPassword").Should().BeTrue();
        user.Password.Verify("newPassword").Should().BeFalse();
    }
    [Fact]
    public void User_ChangePassword_Should_Throw_If_OldPassword_And_NewPassword_Are_Equal()
    {
        //assign
        string newPassword = "oldPassword";
        var user = UserMother.DefaultUser("oldPassword");
        //act
        var act = () => user.ChangePassword("oldPassword", newPassword);
        // assert
        act.Should().Throw<ArgumentException>();
    }

}
