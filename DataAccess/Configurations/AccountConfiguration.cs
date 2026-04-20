using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(a => a.Balance)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasOne(a => a.Parent)
                .WithMany(a => a.Children)
                .HasForeignKey(a => a.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Transactions)
                .WithOne(t => t.Account)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.TransfersFrom)
                .WithOne(t => t.FromAccount)
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.TransfersTo)
                .WithOne(t => t.ToAccount)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.FamilyMember)
                .WithMany(fm => fm.Accounts)
                .HasForeignKey(a => a.FamilyMemberId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.User)
               .WithMany(u => u.Accounts)
               .HasForeignKey(a => a.UserId)
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();

            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.ParentId);
            builder.HasIndex(a => a.FamilyMemberId);

            builder.ToTable(t => t.HasCheckConstraint("CK_Account_Balance", "`Balance` >= 0"));
        }
    }
}
