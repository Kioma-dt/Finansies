using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
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

            builder.HasOne(t => t.FromAccount)
                .WithMany(a => a.TransfersFrom)
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(t => t.ToAccount)
                .WithMany(a => a.TransfersTo)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Transfers)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasIndex(t => t.ToAccountId);
            builder.HasIndex(t => t.FromAccountId);
            builder.HasIndex(t => t.UserId);

            builder.ToTable(t => t.HasCheckConstraint("CK_Transfer_Amount", "`Amount` >= 0"));
        }
    }
}
