namespace Application.Common.DTOs.Tags;
public record TagResponse(
    Guid Id,
    string Name,
    int PostCount
);