using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;
public record Error(string Code,string Message,ErrorType ErrorType)
{
    
    public static Error None() => new(string.Empty,string.Empty,ErrorType.None);
    public static Error Failure(string code,string message) => new(code,message,ErrorType.Failure);
    public static Error Validation(string code,string message) => new(code,message,ErrorType.Validation);
    public static Error NotFound(string code,string message) => new(code,message,ErrorType.NotFound);
    public static Error Conflict(string code,string message) => new(code,message,ErrorType.Conflict);
    public static Error Unauthorized(string code,string message) => new(code,message,ErrorType.Unauthorized);
    public static Error Forbidden(string code,string message) => new(code,message,ErrorType.Forbidden);
}

public enum ErrorType
{
    None,
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden
}
