using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace YourProject.WebAPI.Filters
{
    public class GenericResponseFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Before the action executes

            var executedContext = await next();

            // After the action executes

            IActionResult originalResult = executedContext.Result;
            int? statusCode = (originalResult as IStatusCodeActionResult)?.StatusCode;

            object originalValue = null;

            switch (originalResult)
            {
                case ObjectResult objectResult:
                    originalValue = objectResult.Value;
                    statusCode ??= objectResult.StatusCode;
                    break;
                case JsonResult jsonResult:
                    originalValue = jsonResult.Value;
                    statusCode ??= jsonResult.StatusCode;
                    break;
                case ContentResult contentResult:
                    originalValue = contentResult.Content;
                    statusCode ??= contentResult.StatusCode;
                    break;
                case StatusCodeResult statusCodeResult:
                    statusCode ??= statusCodeResult.StatusCode;
                    break;
                default:
                    break;
            }

            // Initialize the generic response
            var genericResponse = new Dictionary<string, object>
            {
                ["success"] = statusCode >= 200 && statusCode < 300,
                ["data"] = null,
                ["message"] = GetMessageFromStatusCode(statusCode)
            };

            // If there's an error, set the errors property
            if (statusCode >= 400)
            {
                // Check if originalValue contains Errors property
                if (originalValue is IDictionary<string, object> dict && dict.ContainsKey("Errors"))
                {
                    genericResponse["errors"] = dict["Errors"];
                }
                else
                {
                    // If Errors property not found, include originalValue as errors
                    genericResponse["errors"] = originalValue;
                }
            }
            else
            {
                // For success responses, set data
                genericResponse["data"] = originalValue;
            }

            executedContext.Result = new ObjectResult(genericResponse)
            {
                StatusCode = statusCode
            };
        }

        private string GetMessageFromStatusCode(int? statusCode)
        {
            return statusCode switch
            {
                200 => "Operation completed successfully.",
                201 => "Resource created successfully.",
                204 => "No content.",
                400 => "Bad request.",
                401 => "Unauthorized access.",
                403 => "Forbidden.",
                404 => "Resource not found.",
                500 => "An error occurred on the server."            
            };
        }
    }
}