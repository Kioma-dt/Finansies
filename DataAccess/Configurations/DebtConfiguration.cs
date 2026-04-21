using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class DebtConfiguration : IEntityTypeConfiguration<Debt>
    {
        public void Configure(EntityTypeBuilder<Debt> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(d => d.StartAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(d => d.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(d => d.PaidAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(d => d.InterestRate)
                .HasPrecision(12, 4)
                .IsRequired();

            builder.Property(d => d.CapitalisationsPerYear)
                .HasPrecision(12, 4)
                .IsRequired();

            builder.Property(d => d.FixedAddition)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(d => d.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(d => d.InterestType)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(d => d.StartDate)
                .IsRequired();

            builder.Property(d => d.LastPaidDate)
                .IsRequired();

            builder.Property(d => d.EndDate)
                .IsRequired();

            builder.HasOne(d => d.Category)
                .WithMany(c => c.Debts)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.FamilyMember)
                .WithMany(fm => fm.Debts)
                .HasForeignKey(d => d.FamilyMemberId);

            builder.HasOne(d => d.User)
                .WithMany(u => u.Debts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasIndex(d => d.CategoryId);
            builder.HasIndex(d => d.FamilyMemberId);
            builder.HasIndex(d => d.UserId);
            builder.HasIndex(d => d.Type);
            builder.HasIndex(d => d.InterestType);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Debt_Amounts",
                    "`StartAmount` >= 0 AND `TotalAmount` >= 0 AND `PaidAmount` >= 0");

                t.HasCheckConstraint("CK_Debt_Paid_Not_Exceed",
                    "`PaidAmount` <= `TotalAmount`");

                t.HasCheckConstraint("CK_Debt_Dates",
                    "`StartDate` <= `EndDate`");
            });
        }
    }
}
