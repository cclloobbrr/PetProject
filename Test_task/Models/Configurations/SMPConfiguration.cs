using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test_task.Models.Entities;

namespace Test_task.Models.Configurations
{
    public class SMPConfiguration : IEntityTypeConfiguration<SMPEntity>
    {
        public void Configure(EntityTypeBuilder<SMPEntity> builder)
        {
            builder.HasKey(sm => sm.Id);

            builder.Property(sm => sm.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder
                .HasMany(sm => sm.Checks)
                .WithOne(s => s.SMP)
                .HasForeignKey(s => s.SMPId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
