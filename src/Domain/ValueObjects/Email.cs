using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.ValueObjects;
public class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
              @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)+$",
              RegexOptions.Compiled | RegexOptions.CultureInvariant);
    public string Value { get; private set; }

    private Email() { }

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email address");

        Value = email;
    }

    private static bool IsValidEmail(string email)
    {
        if (email.Contains(".."))
            return false;

        return EmailRegex.IsMatch(email);
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
