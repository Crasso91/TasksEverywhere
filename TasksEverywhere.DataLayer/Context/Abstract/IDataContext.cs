using TasksEverywhere.DataLayer.Context.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Context.Abstract
{
    public interface IDataContext<T>
    {
        IConnection Connection { get; set; }
        T data { get; set; }
        void Connect();
        void Init();
        void Commit();
        void Reload();
    }
}
