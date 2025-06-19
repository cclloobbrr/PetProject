using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test_task.Models.Entities;

namespace Test_task.Models.Configurations
{
    public class CheckConfiguration : IEntityTypeConfiguration<CheckEntity>
    {
        public void Configure(EntityTypeBuilder<CheckEntity> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(c => c.SMPId)
                .IsRequired();

            builder.Property(c => c.SupervisoryId)
                .IsRequired();

            builder.Property(c => c.DateStart)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.DateFinish)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.PlannedDuration)
                .IsRequired()
                .HasDefaultValue(0);

            builder
                .HasOne(c => c.Supervisory)
                .WithMany(s => s.Checks)
                .HasForeignKey(s => s.SupervisoryId);

            builder
                .HasOne(c => c.SMP)
                .WithMany(s => s.Checks)
                .HasForeignKey(s => s.SMPId);
        }
    }
}
