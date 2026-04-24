using BuisnessLogic.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMember>
    {
        public void Configure(EntityTypeBuilder<FamilyMember> builder)
        {
            builder.HasKey(fm => fm.Id);

            builder.Property(fm => fm.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasMany(fm => fm.Accounts)
                 .WithOne(a => a.FamilyMember)
                 .HasForeignKey(a => a.FamilyMemberId)
                 .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(fm => fm.Transactions)
                 .WithOne(t => t.FamilyMember)
                 .HasForeignKey(t => t.FamilyMemberId)
                 .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(fm => fm.PlannedTransactions)
                 .WithOne(pt => pt.FamilyMember)
                 .HasForeignKey(pt => pt.FamilyMemberId)
                 .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(fm => fm.Debts)
                 .WithOne(d => d.FamilyMember)
                 .HasForeignKey(d => d.FamilyMemberId)
                 .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(fm => fm.User)
                .WithMany(u => u.FamilyMembers)
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(fm => fm.UserId);
        }
    }
}
