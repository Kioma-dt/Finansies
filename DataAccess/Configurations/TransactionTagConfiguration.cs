using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class TransactionTagConfiguration : IEntityTypeConfiguration<TransactionTag>
    {
        public void Configure(EntityTypeBuilder<TransactionTag> builder)
        {
            builder.HasKey(tt => tt.Id);

            builder.Property(tt => tt.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.HasMany(tt => tt.Transactions)
                .WithMany(t => t.TransactionTags);

            builder.HasMany(tt => tt.PlannedTransactions)
                .WithMany(pt => pt.TransactionTags);

            builder.HasOne(tt => tt.User)
                .WithMany(u => u.TransactionTags)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasIndex(tt => tt.UserId);
        }
    }
}
