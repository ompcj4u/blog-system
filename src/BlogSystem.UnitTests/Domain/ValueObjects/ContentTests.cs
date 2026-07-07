using Domain.ValueObjects;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.UnitTests.Domain.ValueObjects;
public class ContentTests
{
    // happy path
    [Fact]
    public void Content_Should_Create_For_Valid_Input()
    {
        var text = "This is a valid text for content";
        var content = new Content(text);
        content.Value.Should().Be(text);
    }
    [Fact]
    public void Content_Should_Have_Correct_WordCount()
    {
        var content = new Content("this is 5 words totally");
        content.WordCount.Should().Be(5);
    }
    [Fact]
    public void Content_Should_Have_Correct_CharacterCount()
    {
        var content = new Content("This is 26 character count");
        content.CharacterCount.Should().Be(26);
    }
    [Fact]
    public void Content_Should_Truncate_Long_Text_For_Excerpt()
    {
        var text = new string('a', 300);
        var content = new Content(text);
        content.Excerpt.Should().HaveLength(203); // 200 + "..."
        content.Excerpt.Should().EndWith("...");
    }
    [Fact]
    public void Content_Should_Be_Same_As_Excerpt_For_Short_Text()
    {
        var content = new Content("Short Content Equals to Excerpt");
        content.Excerpt.Should().Be("Short Content Equals to Excerpt");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Content_Should_Throw_For_Invalid_Input(string input)
    {
        var action = () => new Content(input);
        action.Should().Throw<ArgumentException>();
    }

}
