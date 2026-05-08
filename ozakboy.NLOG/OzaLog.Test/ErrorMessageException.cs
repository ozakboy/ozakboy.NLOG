using System;

namespace OzaLog
{
    public class ErrorMessageException : Exception
    {
        public ErrorMessageException()
            : base("show message")
        {
            this.HelpLink = "MyServerError";
        }

        public ErrorMessageException(string message)
            : base(message)
        {
            this.HelpLink = "MyServerError";
        }

        public ErrorMessageException(string message, Exception inner)
            : base(message, inner)
        {
            this.HelpLink = "MyServerError";
        }
    }
}
