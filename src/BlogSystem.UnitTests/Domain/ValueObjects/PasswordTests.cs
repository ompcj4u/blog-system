using Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.UnitTests.Domain.ValueObjects;
public class PasswordTests
{
    [Fact]
    public void Password_Should_Create_Both_Hash_And_Salt()
    {
        var passKey = "MyPassword123";
        var password = Password.Create(passKey);
        password.Salt.Should().NotBeNullOrEmpty();
        password.Hash.Should().NotBeNullOrEmpty();
        password.Hash.Should().NotBe(passKey);
    }

    [Fact]
    public void Password_Verify_Returns_True_For_Correct_Password()
    {
        // assign
        var passKey = "MyPassword123";
        // act
        var password = Password.Create(passKey);
        //assert
        password.Verify(passKey).Should().BeTrue();
    }
    [Fact]
    public void Password_Generate_Different_Hash_For_Same_Key()
    {
        var passKey = "SamePassword";
        
        var pass1 = Password.Create(passKey);
        var pass2 = Password.Create(passKey);
        pass1.Hash.Should().NotBe(pass2.Hash);
        pass1.Salt.Should().NotBe(pass2.Salt);
    }

    [Fact]
    public void Password_Verify_Returns_False_For_InCorrect_Password()
    {
        // assign
        var passKey = "MyPassword123";
        var wrongPassKey = "MyPassword456";
        // act
        var password = Password.Create(passKey);
        //assert
        password.Verify(wrongPassKey).Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Password_Throw_For_Invalid_Input(string input)
    {
        var action = () => Password.Create(input);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Password_Throw_For_Short_Input()
    {
        var action = () => Password.Create("12345");
        action.Should().Throw<ArgumentException>();
        
    }

}
