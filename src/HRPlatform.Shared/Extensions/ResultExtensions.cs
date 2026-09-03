using HRPlatform.Shared.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRPlatform.Shared.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this HandlerResult result)
        {
            if (result.Success)
            {
                return new OkObjectResult(new { message = result.Message });
            }

            return MapErrorToActionResult(result.Error);
        }

        public static IActionResult ToActionResult<T>(this HandlerResult<T> result, Func<T, object>? responseMapper = null)
        {
            if (result.Success)
            {
                var responseBody = responseMapper != null && result.Data != null
                    ? responseMapper(result.Data)
                    : (object?)result.Data;

                return new OkObjectResult(responseBody);
            }

            return MapErrorToActionResult(result.Error);
        }

        private static IActionResult MapErrorToActionResult(Error error)
        {
            var problemDetails = new ProblemDetails
            {
                Title = GetTitle(error.Type),
                Detail = error.Description,
                Status = GetStatusCode(error.Type),
                Extensions =
                {
                    ["errorCode"] = error.Code
                }
            };

            if (error.ValidationErrors != null && error.ValidationErrors.Count > 0)
            {
                problemDetails.Extensions["errors"] = error.ValidationErrors;
            }

            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        private static int GetStatusCode(ErrorType type) => type switch
        {
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status400BadRequest
        };

        private static string GetTitle(ErrorType type) => type switch
        {
            ErrorType.Validation => "Unprocessable Entity",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.ServiceUnavailable => "Service Unavailable",
            _ => "Bad Request"
        };
    }
}

