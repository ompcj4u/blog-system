using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Posts;
public record UpdatePostRequest(string Title, string Content, PostStatus Status,List<Guid> Tags);
