namespace Domain.Common;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDateTime { get; set; }
    public DateTime? DeletedDateTime { get; set; }
    public bool IsDeleted { get; set; } = false;
}

