using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test_task.Models.Entities;

namespace Test_task.Models.Configurations
{
    public class SupervisoryConfiguration : IEntityTypeConfiguration<SupervisoryEntity>
    {
        public void Configure(EntityTypeBuilder<SupervisoryEntity> builder)
        {
            builder.HasKey(su => su.Id);

            builder.Property(su => su.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder
                .HasMany(su => su.Checks)
                .WithOne(s => s.Supervisory)
                .HasForeignKey(s => s.SupervisoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
