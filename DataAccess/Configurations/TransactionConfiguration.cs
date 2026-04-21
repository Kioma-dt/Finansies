using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                 .HasPrecision(18, 2)
                 .IsRequired();

            builder.Property(t => t.Description)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(t => t.Date)
                .IsRequired();

            builder.Property(t => t.Type)
                .HasConversion<string>()
                .IsRequired();


            builder.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.TransactionTags)
                .WithMany(tt => tt.Transactions);

            builder.HasOne(t => t.FamilyMember)
                .WithMany(fm => fm.Transactions)
                .HasForeignKey(t => t.FamilyMemberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasIndex(pt => pt.AccountId);
            builder.HasIndex(pt => pt.CategoryId);
            builder.HasIndex(pt => pt.FamilyMemberId);
            builder.HasIndex(pt => pt.UserId);

            builder.ToTable(t => t.HasCheckConstraint("CK_Transaction_Amount", "`Amount` >= 0"));
        }
    }
}
