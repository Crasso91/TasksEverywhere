﻿using System;
using System.Runtime.Serialization;

namespace TasksEverywhere.Api.Dashboard.Exceptions
{
    [Serializable]
    internal class AngularException : Exception
    {
        public override string Message { get; }
        public string Stack { get; set; }

        public AngularException()
        {
        }

        public AngularException(string message, string stack)
        {
            Message = message;
            Stack = stack;
        }

        public AngularException(string message) : base(message)
        {
        }

        public AngularException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AngularException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}