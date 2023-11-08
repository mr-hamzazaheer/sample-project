using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Middleware
{
    //You can overide authorize functionality like this
    public class Authorize : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .Any();

            if (allowAnonymous || (!string.IsNullOrEmpty(context.HttpContext.UserProfile()?.Id) && context.HttpContext.ValidatePermission()))
                await next();
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            return;

        }
    }
}
