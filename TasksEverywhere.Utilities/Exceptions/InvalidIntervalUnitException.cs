using System;
using System.Runtime.Serialization;

namespace TasksEverywhere.Utilities.Exceptions
{
    [Serializable]
    public class InvalidIntervalUnitException : Exception
    {
        public InvalidIntervalUnitException()
        {
        }

        public InvalidIntervalUnitException(string message) : base(message)
        {
        }

        public InvalidIntervalUnitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidIntervalUnitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}