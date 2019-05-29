using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.CastleWindsor.Services.Abstract;
using TasksEverywhere.DataLayer;
using TasksEverywhere.DataLayer.Context.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Context.Concrete
{
    public class QuartzExecutionDataService : IDataContext<QuartzExecutionDataContext>, ITransientService
    {
        public QuartzExecutionDataContext data { get ; set; }
        public IConnection Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public QuartzExecutionDataService()
        {
            this.Init();
        }

        public void Init()
        {
            this.Connect();
        }

        public void Connect()
        {
            try
            {
                data = new QuartzExecutionDataContext();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public void Commit()
        {
            try
            {
                data.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Reload()
        {
            foreach (var entity in data.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }

        public void Dispose()
        {
            if (data != null && data.Database.Connection != null) data.Database.Connection.Dispose();
            if (data != null) data.Dispose();
        }
    }
}
