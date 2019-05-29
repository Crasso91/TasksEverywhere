using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Context.Concrete
{
    internal sealed class Configuration
        : DbMigrationsConfiguration<QuartzExecutionDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(QuartzExecutionDataContext context)
        {

        }
    }
}
