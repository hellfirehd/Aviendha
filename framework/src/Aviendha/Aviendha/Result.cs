// Aviendha ABP Framework Extensions
// Copyright (C) 2025 Doug Wilson
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using System.Text.Json.Serialization;

namespace Aviendha;

/// <summary>
/// The bare minimum response for all API requests.
/// </summary>
public record Result
{
    public const String SuccessMessage = "Succeeded";
    public const String FailureMessage = "Failed";

    /// <summary>
    /// Deserialization Constructor
    /// </summary>
    [JsonConstructor]
    public Result() { }

    protected internal Result(Boolean succeeded, String? message = null, IEnumerable<Error>? errors = null)
    {
        Succeeded = succeeded;
        Message = message ?? (succeeded ? SuccessMessage : FailureMessage);

        Errors = errors?.ToArray() ?? [];
    }

    public String ErrorMessage => Succeeded ? String.Empty : Message;

    public Boolean Succeeded { get; init; }
    public Boolean Failed => !Succeeded;

    public String Message { get; init; } = String.Empty;
    public IEnumerable<Error> Errors { get; init; } = [];

    [return: NotNull]
    public static Result Failure(ErrorCode errorCode, String message)
        => new(succeeded: false, null, [new Error(errorCode, message)]);

    [return: NotNull]
    public static Result Failure(Error error)
        => new(succeeded: false, null, [error]);

    [return: NotNull]
    public static Result Failure(String message, Error error)
        => new(succeeded: false, message, [error]);

    [return: NotNull]
    public static Result Failure(String message, IEnumerable<Error>? errors = null)
        => new(succeeded: false, message, errors);

    [return: NotNull]
    public static Result<T> Failure<T>(Error error)
        => new(succeeded: false, default, null, [error]);

    [return: NotNull]
    public static Result<T> Failure<T>(String message, Error error)
        => new(succeeded: false, default, message, [error]);

    [return: NotNull]
    public static Result<T> Failure<T>(String message, IEnumerable<Error>? errors = null)
        => new(succeeded: false, default, message, errors);

    [return: NotNull]
    public static Result Success() => new(succeeded: true);

    [return: NotNull]
    public static Result Success(String? message = null)
        => new(succeeded: true, message);

    [return: NotNull]
    public static Result<T> Success<T>(T? value, String? message = null)
        => new(succeeded: true, value, message);
}

/// <summary>
/// The response from all API requests that return a <typeparamref name="TDto"/> result.
/// </summary>
public record Result<TDto> : Result
{
    /// <summary>
    /// Deserialization Constructor
    /// </summary>
    [JsonConstructor]
    public Result() { }

    public Result(Boolean succeeded, TDto? value, String? message = null, IEnumerable<Error>? errors = null)
        : base(succeeded, message, errors) => Value = value ?? default!;

    /// <summary>
    /// Gets the value returned int the response. Make sure you check <see cref="Result.Succeeded"/> or you may end up with a <see cref="NullReferenceException"/>.
    /// </summary>
    public TDto Value { get; init; } = default!;
}
