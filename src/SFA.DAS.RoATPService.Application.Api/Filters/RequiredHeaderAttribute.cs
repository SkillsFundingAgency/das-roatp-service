namespace SFA.DAS.RoATPService.Application.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RequiredHeaderAttribute : ActionFilterAttribute
{
    private readonly string _headerName;

    public RequiredHeaderAttribute(string headerName)
    {
        _headerName = headerName;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Check if the request headers contain the required header name
        if (!context.HttpContext.Request.Headers.TryGetValue(_headerName, out var headerValue))
        {
            // If the header is missing, set the result to a 400 Bad Request
            context.Result = new BadRequestObjectResult(new
            {
                Error = $"Missing required header: '{_headerName}'."
            });
            return;
        }

        // Check if the request header is null or empty
        if (string.IsNullOrEmpty(headerValue))
        {
            // If the header is null or empty, set the result to a 400 Bad Request
            context.Result = new BadRequestObjectResult(new
            {
                Error = $"Required header is empty: '{_headerName}'."
            });
            return;
        }

        base.OnActionExecuting(context);
    }
}