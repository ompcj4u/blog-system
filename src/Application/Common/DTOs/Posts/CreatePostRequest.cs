using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Posts;
public record CreatePostRequest(string Title, string Content, List<Guid> Tags);
