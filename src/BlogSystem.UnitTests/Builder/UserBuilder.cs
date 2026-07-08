using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.UnitTests.Builder;
public class UserBuilder
{
    private string fullName = "full name";
    private Email email;
    private Password password;

    private string token;
    private DateTime expireDateTime = DateTime.MinValue;

    public UserBuilder()
    {
        email = new Email("mohammad@gmail.com");
        password = Password.Create("123456");
    }

    public UserBuilder WithFullName(string fullName)
    {
        this.fullName = fullName; return this;
    }
    public UserBuilder WithEmail(string email) { this.email = new Email(email); return this; }
    public UserBuilder WithEmail(Email email) { this.email = email; return this; }
    public UserBuilder WithPassword(string password) { this.password = Password.Create(password); return this; }
    public UserBuilder WithPassword(Password password) { this.password = password; return this; }

    public UserBuilder WithRefreshToken(string refreshToken, DateTime expiry)
    {
        token = refreshToken;
        expireDateTime = expiry;
        return this;
    }

    public User Build()
    {
        var user = new User(fullName, email, password);
        if (expireDateTime > DateTime.MinValue)
            user.SetRefreshToken(token, expireDateTime);
        return user;
    }

}
