using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppAPI.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Log the exception here if needed

            var genericResponse = new
            {
                success = false,
                data = (object)null,
                message = "An unexpected error occurred.",

            };

            context.Result = new ObjectResult(genericResponse)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
