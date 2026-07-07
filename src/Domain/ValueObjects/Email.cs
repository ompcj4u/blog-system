using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects;
public class Email : ValueObject
{
    public string Value { get; private set; }

    private Email() { }

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");

        email = email.Trim().ToLowerInvariant();

        if(MailAddress.TryCreate(email, out _))
            throw new ArgumentException("Invalid email address");



        Value = email;
    }

    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
