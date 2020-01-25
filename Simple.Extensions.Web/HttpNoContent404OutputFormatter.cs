using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Simple.Extensions.Web
{
    public class HttpNoContent404OutputFormatter : IOutputFormatter
    {
        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context.ObjectType == typeof(void) || context.ObjectType == typeof(Task))
            {
                return true;
            }

            return context.Object == null;
        }

        public Task WriteAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentLength = 0;

            if (response.StatusCode == StatusCodes.Status200OK)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
            }

            return Task.CompletedTask;
        }
    }
}
