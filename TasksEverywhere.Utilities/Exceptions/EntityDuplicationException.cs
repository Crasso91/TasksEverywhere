using System;
using System.Runtime.Serialization;

namespace TasksEverywhere.Utilities.Exceptions
{
    [Serializable]
    public class EntityDuplicationException : Exception
    {
        public EntityDuplicationException()
        {
        }

        public EntityDuplicationException(string message) : base(message)
        {
        }

        public EntityDuplicationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntityDuplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}