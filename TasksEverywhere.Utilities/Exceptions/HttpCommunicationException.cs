using System;
using System.Runtime.Serialization;

namespace TasksEverywhere.Utilities.Exceptions
{
    [Serializable]
    public class HttpCommunicationException : Exception
    {
        public HttpCommunicationException()
        {
        }

        public HttpCommunicationException(string message) : base(message)
        {
        }

        public HttpCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpCommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}