using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TasksEverywhere.DataLayer.Context.Concrete
{
    public class QuartzExecutionDataContext : DbContext
    {
        public QuartzExecutionDataContext() : base("QuartzExecutionDataContext")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<QuartzExecutionDataContext>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<QuartzExecutionDataContext, Configuration>());
        }
        public DbSet<Models.Server> Servers { get; set; }
        public DbSet<Models.Job> Jobs { get; set; }
        public DbSet<Models.Call> Calls { get; set; }
        public DbSet<Models.Error> Errors { get; set; }
        public DbSet<Models.Account> Accounts { get; set; }
        public DbSet<Models.AccountServer> AccountServers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Account>()
                .HasMany(x => x.Servers)
                .WithMany(x => x.Accounts)
                .Map(rel =>
                {
                    rel.MapLeftKey("AccountID");
                    rel.MapRightKey("ServerID");
                    rel.ToTable("AccountServers");
                });

            modelBuilder.Entity<Models.Server>()
                .HasMany(x => x.Jobs)
                .WithRequired(x => x.Server)
                .WillCascadeOnDelete(true);


            modelBuilder.Entity<Models.Job>()
                .HasMany(x => x.Calls)
                .WithRequired(x => x.Job)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Models.Call>()
                .HasOptional(x => x.Error)
                .WithRequired(x => x.Call);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

    }
}
