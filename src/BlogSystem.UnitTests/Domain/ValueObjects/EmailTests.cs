using Domain.ValueObjects;
using FluentAssertions;

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
    [InlineData("mohammad@gmail.co.uk", "mohammad@gmail.co.uk")]
    public void Email_Should_Trim_WhiteSpace(string input,string expected)
    {
        //act
        var email = new Email(input);
        // assert
        email.Value.Should().Be(expected);
    }

    /// <summary>
    /// firstEmail and secondEmail are repetetive
    /// we can use BuilderPattern or AutoFixture
    /// </summary>

    [Fact]
    public void Email_Should_Be_Equal_When_Values_Are_Equal()
    {
        var firstEmail = new Email("mohammad@gmail.com");
        var secondEmail = new Email("Mohammad@gmail.com");
        firstEmail.Should().Be(secondEmail); // check value
    }
    [Fact]
    public void Email_Should_Not_Be_Same_Instance_When_Values_Are_Equal()
    {
        var firstEmail = new Email("mohammad@gmail.com");
        var secondEmail = new Email("Mohammad@gmail.com");
        firstEmail.Should().NotBeSameAs(secondEmail); // check reference
    }
    [Fact]
    public void Email_Should_Have_Same_HashCode_When_Values_Are_Equal()
    {
        var firstEmail = new Email("mohammad@gmail.com");
        var secondEmail = new Email("Mohammad@gmail.com");
        secondEmail.GetHashCode().Should().Be(firstEmail.GetHashCode());
    }


    // sad path
    [Theory]
    [InlineData("  ")]
    [InlineData("mohammad@gmail.")]
    [InlineData("mohammad@gmail..")]
    [InlineData("mohammad@.com")]
    [InlineData("  @gmail.com")]
    [InlineData("mohammad")]
    [InlineData("mohammad@gmail..com")]
    public void Email_Should_Throw_Exception_For_Invalid_Emails(string input)
    {
        var act = () => new Email(input);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Email_Should_Throw_For_Null_Empty_Email()
    {
        var act = () => new Email(null);
        act.Should().Throw<ArgumentException>();
    }
    [Fact]
    public void Email_Should_Throw_For_Empty_Email()
    {
        var act = () => new Email("");
        act.Should().Throw<ArgumentException>();
    }

    // [MemberData(nameof(InvalidEmails))]
    public static IEnumerable<string> InvalidEmails => [

                 "mohammad@gmail.",
                 "mohammad@gmail.."
            ];

}
