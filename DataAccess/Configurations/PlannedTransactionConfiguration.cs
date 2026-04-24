using BuisnessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class PlannedTransactionConfiguration : IEntityTypeConfiguration<PlannedTransaction>
    {
        public void Configure(EntityTypeBuilder<PlannedTransaction> builder)
        {
            builder.HasKey(pt=> pt.Id);

            builder.Property(pt => pt.Amount)
                 .HasPrecision(18, 2)
                 .IsRequired();

            builder.Property(pt => pt.Description)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(pt => pt.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(pt => pt.PlannedDate)
                .IsRequired();

            builder.Property(pt => pt.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.HasOne(pt => pt.Account)
                .WithMany(a => a.PlannedTransactions)
                .HasForeignKey(pt => pt.AccountId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(pt => pt.Category)
                .WithMany(c => c.PlannedTransactions)
                .HasForeignKey(pt => pt.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(pt => pt.TransactionTags)
                .WithMany(tt => tt.PlannedTransactions);

            builder.HasOne(pt => pt.FamilyMember)
                .WithMany(fm =>  fm.PlannedTransactions)
                .HasForeignKey(pt => pt.FamilyMemberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(pt => pt.User)
                .WithMany(u => u.PlannedTransactions)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasIndex(pt => pt.AccountId);
            builder.HasIndex(pt => pt.CategoryId);
            builder.HasIndex(pt => pt.FamilyMemberId);
            builder.HasIndex(pt => pt.UserId);

            builder.ToTable(t =>  t.HasCheckConstraint("CK_PlannedTransaction_Amount", "`Amount` >= 0"));
        }
    }
}
