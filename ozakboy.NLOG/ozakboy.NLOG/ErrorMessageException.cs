using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ozakboy.NLOG
{
    public class ErrorMessageException : Exception, ISerializable
    {
        public ErrorMessageException()
         : base("show message")
        {
            this.HelpLink = "MyServerError";
        }
        public ErrorMessageException(string message)
            : base(message) { this.HelpLink = "MyServerError"; }    
        public ErrorMessageException(string message, Exception inner)
            : base(message, inner) { this.HelpLink = "MyServerError"; }
        protected ErrorMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context) { this.HelpLink = "MyServerError"; }

    }
}
