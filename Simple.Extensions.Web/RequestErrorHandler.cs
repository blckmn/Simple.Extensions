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

        public RequestErrorHandler(ILogger<RequestErrorHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleError(HttpContext context)
        {
            var response = context.Response;
            response.StatusCode = 500;
            response.ContentType = "text/html";

            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception != null)
            {
                var message = exception.Message;
                var id = Guid.NewGuid();

                switch (exception)
                {
                    case BadFormatException _:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case NoPermissionException _:
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case NotFoundException _:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        _logger?.LogError(exception, message);
                        message = $"An error occurred. Reference: {id}";
                        break;
                }

                await response.WriteAsync($"{message}");
            }

            await response.WriteAsync(new string(' ', 512)); // IE padding
            await response.WriteAsync("\n\n\n");
        }
    }

    public class BadFormatException : Exception
    {
        public BadFormatException(string message) : base(message) { }
    }

    public class NoPermissionException : Exception
    {
        public NoPermissionException(string message) : base(message) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
