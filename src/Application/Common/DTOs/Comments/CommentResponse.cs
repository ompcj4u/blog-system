using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Comments;
public record CommentResponse(Guid Id, string AuthorName, string Text, DateTime CreatedAt);
