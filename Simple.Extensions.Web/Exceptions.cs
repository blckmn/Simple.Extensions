using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.Extensions.Web
{
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
        public NotFoundException() : base() { }
        public NotFoundException(string message) : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }

    public class CustomHttpResponseException : Exception
    {
        public int Code { get; set; }
        public CustomHttpResponseException(int code, string message) : base(message)
        {
            Code = code;
        }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException() { }
        public ForbiddenException(string message) : base(message) { }
    }

    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException() { }
        public UnAuthorizedException(string message) : base(message) { }
    }
}
