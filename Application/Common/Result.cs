using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common
{
    public record Result(bool IsSuccess, string? ErrorMessage)
    {
        public bool IsFailure => !IsSuccess;

        public static Result Success() => new(true, null);
        public static Result Failure(string errorMessage) => new(false, errorMessage);
    }

    public record Result<T>(T? Value, bool IsSuccess, string? ErrorMessage)
        : Result(IsSuccess, ErrorMessage)
    {
        public static Result<T> Success(T value) => new(value, true, null);
        public static new Result<T> Failure(string errorMessage) => new(default, false, errorMessage);
    }
}
