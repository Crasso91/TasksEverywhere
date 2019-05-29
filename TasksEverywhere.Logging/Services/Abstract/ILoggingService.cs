using TasksEverywhere.CastleWindsor.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.Logging.Services.Abstract
{
    public interface ILoggingService : ISingletonService
    {
        bool Active { get; set; }
        void Configure();
        void Debug(Type type, MethodBase method, string message);
        void Error(Type type, MethodBase method, string message, Exception ex);
        void Info(Type type, MethodBase method, string message);
    }
}
