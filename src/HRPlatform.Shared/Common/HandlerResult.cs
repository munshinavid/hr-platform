namespace HRPlatform.Shared.Common
{
    public enum ErrorType
    {
        Failure = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        ServiceUnavailable = 6
    }

    public sealed record Error(
        string Code,
        string Description,
        ErrorType Type = ErrorType.Failure,
        IDictionary<string, string[]>? ValidationErrors = null)
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public static Error Failure(string code, string description) =>
            new(code, description, ErrorType.Failure);

        public static Error Validation(string code, string description, IDictionary<string, string[]>? validationErrors = null) =>
            new(code, description, ErrorType.Validation, validationErrors);

        public static Error NotFound(string code, string description) =>
            new(code, description, ErrorType.NotFound);

        public static Error Conflict(string code, string description) =>
            new(code, description, ErrorType.Conflict);

        public static Error Unauthorized(string code, string description) =>
            new(code, description, ErrorType.Unauthorized);

        public static Error Forbidden(string code, string description) =>
            new(code, description, ErrorType.Forbidden);

        public static Error ServiceUnavailable(string code, string description) =>
            new(code, description, ErrorType.ServiceUnavailable);
    }

    public class HandlerResult
    {
        private string? _message;

        public bool Success { get; protected set; }

        public string? Message
        {
            get => Error != null && !string.IsNullOrEmpty(Error.Description) ? Error.Description : _message;
            protected set => _message = value;
        }

        public Error Error { get; protected set; } = Error.None;

        public static HandlerResult SuccessResult(string? message = null)
        {
            return new HandlerResult
            {
                Success = true,
                Message = message,
                Error = Error.None
            };
        }

        public static HandlerResult FailureResult(string message)
        {
            return new HandlerResult
            {
                Success = false,
                Message = message,
                Error = Error.Failure("GENERAL_ERROR", message)
            };
        }

        public static HandlerResult FailureResult(Error error)
        {
            return new HandlerResult
            {
                Success = false,
                Message = error.Description,
                Error = error
            };
        }

        public void Fail(Error error)
        {
            Success = false;
            Message = error.Description;
            Error = error;
        }
    }

    public class HandlerResult<T> : HandlerResult
    {
        public T? Data { get; private set; }

        public static HandlerResult<T> SuccessResult(T data, string? message = null)
        {
            return new HandlerResult<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Error = Error.None
            };
        }

        public new static HandlerResult<T> FailureResult(string message)
        {
            return new HandlerResult<T>
            {
                Success = false,
                Message = message,
                Error = Error.Failure("GENERAL_ERROR", message)
            };
        }

        public new static HandlerResult<T> FailureResult(Error error)
        {
            return new HandlerResult<T>
            {
                Success = false,
                Message = error.Description,
                Error = error
            };
        }

    }
}
