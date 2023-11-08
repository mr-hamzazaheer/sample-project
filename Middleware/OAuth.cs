global using Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;


namespace Middleware
{
    public class OAuth
    {
        private readonly RequestDelegate _next;
        private IWebHostEnvironment _hostingEnvironment { get; }
        private readonly ILogger<OAuth> _logger;
        private Response _response;
        public OAuth(RequestDelegate next,
            IWebHostEnvironment hostingEnvironment, ILogger<OAuth> logger)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;

        }

        public async Task Invoke(HttpContext context, Response response)
        {

            try
            {
                _response = response;
                string authHeader = context.Request.Headers["auth"];
                switch (authHeader)
                {
                    case "":
                    case null:
                        {
                            if (context.Request.Path.Value.ShouldBypassMiddleware())
                                await context.Response.Body.WriteAsync(File.ReadAllBytes(_hostingEnvironment.WebRootPath + context.Request.Path.Value));
                            else
                                await context.Response.WriteAsync(Extension.InvalidRequest());
                            break;
                        }
                    default:
                        {
                            authHeader = authHeader.Decode();
                            if ((authHeader.Substring(0, authHeader.IndexOf(':')) == Static.Settings.AuthCredential.ClientId &&
                                (authHeader.Substring(authHeader.IndexOf(':') + 1)) == Static.Settings.AuthCredential.ClientSecret))
                                await _next.Invoke(context);
                            else
                                await context.Response.WriteAsync(Extension.InvalidRequest());
                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            return;
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            switch (exception)
            {
                case ApplicationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    _response.Message = ex.Message;
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    _response.Message = ex.Message;
                    break;
                case BadHttpRequestException ex:
                    response.StatusCode = (int)HttpStatusCode.Redirect;
                    _response.Message = ex.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    _response.Message = Message.Error;
                    break;
            }
            _response.StatusCode = (HttpStatusCode)context.Response.StatusCode;

            while (exception.InnerException != null) exception = exception.InnerException;
            _logger.LogError("{msg} on line# {excep}", exception.Message, exception);
            await context.Response.WriteAsync(_response.Serialize());
        }
    }
}
