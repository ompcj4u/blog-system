using Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.UnitTests.Domain.ValueObjects;
public class EmailTests
{
    // happy path
    [Fact]
    public void Email_Should_Create_When_Email_Is_Valid()
    {
        // arrange
        var emailAddress = "mohammad@gmail.com";
        // act
        var email = new Email(emailAddress);
        //assert
        email.Value.Should().Be(emailAddress);
    }

    // happy path
    [Theory]
    [InlineData(" mohammad@gmail.com ", "mohammad@gmail.com")]
    [InlineData("Mohammad@gmail.com", "mohammad@gmail.com")]
    [InlineData("mohammad@gmail.com", "mohammad@gmail.com")]
    public void Email_Should_Trim_WhiteSpace(string input,string expected)
    {
        //act
        var email = new Email(input);
        // assertion
        email.Value.Should().Be(expected);
    }

    // sad path
    [Theory]
    [InlineData("  ")]
    [InlineData("mohammad@gmail.")]
    [InlineData("mohammad@.com")]
    [InlineData("  @gmail.com")]
    [InlineData("mohammad")]
    public void Email_Should_Throw_Exception_For_Invalid_Emails(string input)
    {
        var act = () => new Email(input);
        act.Should().Throw<ArgumentException>().WithMessage("Invalid email format");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Email_Should_Throw_Exceptions_For_Null_Empty_Email(string input)
    {
        var act = () => new Email(input);
        act.Should().Throw<ArgumentException>().WithMessage("Email cannot be empty");
    }


}
