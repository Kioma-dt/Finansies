using BuisnessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(b =>  b.Id);

            builder.Property(b => b.Name)
               .IsRequired()
               .HasMaxLength(128);

            builder.Property(b => b.Limit)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(b => b.StartDate)
                .IsRequired();

            builder.Property(b => b.EndDate)
                .IsRequired();

            builder.HasMany(b => b.Filters)
                .WithOne(bt => bt.Budget)
                .HasForeignKey(bt => bt.BudgetId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(b => b.User)
               .WithMany(u => u.Budgets)
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

            builder.HasIndex(b => b.UserId);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Budget_Limit",
                    "`Limit` >= 0");

                t.HasCheckConstraint("CK_Budget_Dates",
                    "`StartDate` <= `EndDate`");
            });
        }
    }
}
