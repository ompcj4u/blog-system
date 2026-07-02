using Application.Common;
using Application.Common.DTOs.Tags;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Tag.Queries;
public record GetAllTagsQuery : IRequest<Result<List<TagResponse>>>;

public class GetAllTagsQueryHandler(ITagRepository _tagRepo) : IRequestHandler<GetAllTagsQuery, Result<List<TagResponse>>>
{
    public async Task<Result<List<TagResponse>>> Handle(GetAllTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepo.GetAllAsync(t => true);

        var response = tags.Select(t => new TagResponse(
            t.Id,
            t.TagName,
            t.PostTags?.Count ?? 0
        )).ToList();

        return Result<List<TagResponse>>.Success(response, "list of Tags");
    }
}
