using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;
public class Result
{
    internal Result() { IsSuccess = false; Message = string.Empty; }
    public bool IsSuccess { get; internal set; }
    public string Message { get; internal set; }

    public static Result Success(string message = "Success") => new Result() { IsSuccess = true, Message = message };
    public static Result Fail(string message) => new Result() { IsSuccess = false, Message = message };
}

public class Result<T> : Result
{
    public T Value { get; private set; }
    public static Result<T> Success(T value, string message) => new Result<T>() { IsSuccess = true, Value = value, Message = message };
    public static Result<T> Fail(string message) => new Result<T>() { IsSuccess = false, Message = message };
}

public class PaginatedResult<T> : Result
{
    public PaginatedList<T> Data { get; private set; }

    private PaginatedResult() { }

    public static PaginatedResult<T> Success(PaginatedList<T> data, string message = "Success")
    {
        return new PaginatedResult<T> { IsSuccess = true, Data = data, Message = message };
    }
}
