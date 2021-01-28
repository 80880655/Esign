using System;
using System.Runtime.Serialization;

namespace Comfy.Utils.Core
{
    public class AppException : Exception
    {
        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        protected AppException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public AppException(string message, Exception innerException) : base(message, innerException) { }
    }
}
