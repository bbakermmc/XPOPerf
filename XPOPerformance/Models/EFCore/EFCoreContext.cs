using Microsoft.EntityFrameworkCore;

using System.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ORMBenchmark.Models.EFCore {
    public partial class EFCoreContext : DbContext {
        
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[]
            {
                new NLogLoggerProvider()
            });
        public virtual DbSet<EFCoreEntity> Entities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if(!optionsBuilder.IsConfigured) {
                string connstr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                optionsBuilder.UseSqlServer(connstr, o => o.UseRowNumberForPaging(true));//.UseLoggerFactory(MyLoggerFactory);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<EFCoreEntity>(entity => {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
            modelBuilder.Entity<EFCoreEntity>().ToTable("Entities");
        }
    }
}
