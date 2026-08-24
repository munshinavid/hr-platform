namespace EmployeeManagement.Handler.Common
{
    public class HandlerResult
    {
        public bool Success { get; protected set; }

        public string? Message { get; protected set; }

        public static HandlerResult SuccessResult(string? message = null)
        {
            return new HandlerResult { Success = true, Message = message };
        }

        public static HandlerResult FailureResult(string message)
        {
            return new HandlerResult { Success = false, Message = message };
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
                Message = message
            };
        }

        public new static HandlerResult<T> FailureResult(string message)
        {
            return new HandlerResult<T>
            {
                Success = false,
                Message = message
            };
        }
    }
}
