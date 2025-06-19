using Microsoft.EntityFrameworkCore;
using Test_task.Models.Configurations;
using Test_task.Models.Entities;

namespace Test_task.Data
{
    public class Test_taskDbContext(DbContextOptions<Test_taskDbContext> options) : DbContext(options)
    {
        public DbSet<CheckEntity> Checks { get; set; }
        public DbSet<SMPEntity> SMP { get; set; }
        public DbSet<SupervisoryEntity> Supervisory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CheckConfiguration());
            modelBuilder.ApplyConfiguration(new SMPConfiguration());
            modelBuilder.ApplyConfiguration(new SupervisoryConfiguration());


            base.OnModelCreating(modelBuilder);
        }
    }
}
