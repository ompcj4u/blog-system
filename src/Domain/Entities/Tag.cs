using Domain.Common;

namespace Domain.Entities;
public class Tag : BaseEntity
{
    private Tag() { }

    public Tag(string title)
    {
        TagName = title;
    }
    public string TagName { get; set; }

    public IReadOnlyCollection<PostTag> PostTags { get; set; }

}
