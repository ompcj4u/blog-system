using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;
public class User : BaseEntity
{
    // for entityFramework
    private User() { }

    public User(string fullName, Email email, Password password)
    {
        FullName = fullName;
        Email = email;
        Password = password;
    }
    public string FullName { get; set; }
    public string RefreshToken { get;set; }
    public DateTime RefreshTokenExpiry { get;set; }
    public Email Email { get; set; }
    public Password Password { get; set; }

    public IReadOnlyCollection<Post> Posts { get; set; } = new List<Post>();
    public IReadOnlyCollection<Comment> Comments { get; set; } = new List<Comment>();
    public IReadOnlyCollection<PostLike> LikedPosts { get; set; } = new List<PostLike>();

    public void SetRefreshToken(string refreshToken, DateTime dateTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = dateTime;
        ModifiedDateTime = DateTime.UtcNow;
    }
}
