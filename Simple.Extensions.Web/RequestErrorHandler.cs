using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Simple.Extensions.Web
{
    public static class RequestErrorHandler
    {
        public static async Task Handler(HttpContext context)
        {
            var response = context.Response;
            response.StatusCode = 500;
            response.ContentType = "text/html";

            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception != null)
            {
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
                }

                await response.WriteAsync($"{exception.Message}");
            }

            await response.WriteAsync(new string(' ', 512)); // IE padding
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
