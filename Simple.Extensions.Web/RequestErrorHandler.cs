using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Simple.Extensions.Web
{
    public class RequestErrorHandler
    {
        private readonly ILogger<RequestErrorHandler> _logger;

        public RequestErrorHandler()
        {
            _logger = null;
        }
        
        public RequestErrorHandler(ILogger<RequestErrorHandler> logger)
        {
            _logger = logger;
        }

        private string DefaultAction(Exception exception, HttpResponse response)
        {
            var id = Guid.NewGuid();
            _logger?.LogError(exception, exception.Message);
            return $"An error occurred. Reference: {id}";
        }

        public async Task HandleError(HttpContext context)
        {
            await HandleError(context, DefaultAction);
        }

        public async Task HandleError(HttpContext context, Func<Exception, HttpResponse, string> action)
        {
            var response = context.Response;
            response.StatusCode = 500;
            response.ContentType = "text/html";

            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception != null)
            {
                var message = exception.Message;

                switch (exception)
                {
                    case BadFormatException _:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case UnAuthorizedException _:
                    case NoPermissionException _:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case ForbiddenException _:
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        break;
                    case NotFoundException _:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ConflictException _:
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        break;
                    case CustomHttpResponseException e:
                        response.StatusCode = e.Code;
                        break;
                    default:
                        message = action?.Invoke(exception, response);
                        break;
                }

                if (message != null)
                {
                    await response.WriteAsync($"{message}");
                }
            }

            await response.WriteAsync(new string(' ', 512)); // IE padding
            await response.WriteAsync("\n\n\n");
        }
    }
}
