using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Utilities.Exceptions
{
    public class InvalidLifeUnitException : Exception
    {

        public InvalidLifeUnitException()
        {
        }

        public InvalidLifeUnitException(string message) : base(message)
        {
        }

        public InvalidLifeUnitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLifeUnitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
