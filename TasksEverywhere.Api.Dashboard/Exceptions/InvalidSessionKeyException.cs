using System;
using System.Runtime.Serialization;

namespace TasksEverywhere.Api.Dashboard.Exceptions
{
    [Serializable]
    internal class InvalidSessionKeyException : Exception
    {
        public InvalidSessionKeyException()
        {
        }

        public InvalidSessionKeyException(string message) : base(message)
        {
        }

        public InvalidSessionKeyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSessionKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}