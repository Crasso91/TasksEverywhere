using TasksEverywhere.DataLayer.Context.Abstract;
using TasksEverywhere.Extensions;
using System.Linq;

namespace TasksEverywhere.DataLayer.Context.Concrete
{
    public class BaseConnection : IConnection
    {
        public T GetProperty<T>(string key)
        {
            return this.GetType().GetProperties().First(x => x.Name == key).GetValue(this).ToType<T>();
        }
    }
}