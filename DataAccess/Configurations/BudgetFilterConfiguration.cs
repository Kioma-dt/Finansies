using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class BudgetFilterConfiguration : IEntityTypeConfiguration<BudgetFilter>
    {
        public void Configure(EntityTypeBuilder<BudgetFilter> builder)
        {
            builder.HasKey(bf => bf.Id);

            builder.Property(bf => bf.Value)
               .IsRequired()
               .HasMaxLength(128);

            builder.Property(bf => bf.Type)
                 .HasConversion<string>()
                 .IsRequired();

            builder.HasOne(bf => bf.Budget)
                .WithMany(b => b.Filters)
                .HasForeignKey(bt => bt.BudgetId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();


            builder.HasIndex(bf => bf.BudgetId);
        }
    }
}
