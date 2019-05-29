using System;
using System.Runtime.Serialization;

namespace TasksEverywhere.Utilities.Exceptions
{
    [Serializable]
    public class EntityNotExistsException : Exception
    {
        public EntityNotExistsException()
        {
        }

        public EntityNotExistsException(string message) : base(message)
        {
        }

        public EntityNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntityNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}