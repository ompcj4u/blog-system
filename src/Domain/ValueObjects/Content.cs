using Domain.Common;

namespace Domain.ValueObjects;
public class Content : ValueObject
{
    public string Value { get; private set; }
    public int WordCount => Value.Split(' ').Length;
    public int CharacterCount => Value.Length;

    public string Excerpt => Value.Length > 200 ? Value[..200] + "..." : Value;
    private Content() { }

    public Content(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Content cannot be empty");

        if (value.Length < 10)
            throw new ArgumentException("Content must be at least 10 characters");

        if (value.Length > 50000)
            throw new ArgumentException("Content cannot exceed 50000 characters");

        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
