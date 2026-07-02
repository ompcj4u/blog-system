using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects;
public class Password : ValueObject
{
    public string Salt { get; private set; }
    public string Hash { get; private set; }

    private Password() { }

    public Password(string hash, string salt)
    {
        Salt = salt;
        Hash = hash;
    }

    public static Password Create(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("رمز عبور نمی‌تواند خالی باشد");

        if (plainPassword.Length < 6)
            throw new ArgumentException("رمز عبور حداقل باید 6 کاراکتر باشد");

        if (plainPassword.Length > 100)
            throw new ArgumentException("رمز عبور حداکثر 100 کاراکتر می‌تواند باشد");

        // تولید Salt
        byte[] saltBytes = RandomNumberGenerator.GetBytes(32);
        string salt = Convert.ToBase64String(saltBytes);

        // هش کردن با Salt
        string hash = HashPassword(plainPassword, salt);

        return new Password(hash, salt);

    }

    public static Password FromDataBase(string hash, string salt)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash نمی‌تواند خالی باشد");

        if (string.IsNullOrWhiteSpace(salt))
            throw new ArgumentException("Salt نمی‌تواند خالی باشد");

        return new Password(hash, salt);
    }

    public bool Verify(string plainPassword)
    {
        string hash = HashPassword(plainPassword, Salt);
        return hash == Hash;
    }

    private static string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
        byte[] hashBytes = sha256.ComputeHash(combinedBytes);
        return Convert.ToBase64String(hashBytes);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
        yield return Salt;
    }
}
