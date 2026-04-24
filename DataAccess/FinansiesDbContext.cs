using Microsoft.EntityFrameworkCore;
using BuisnessLogic.Entities;
using DataAccess.Configurations;

namespace DataAccess
{
    public class FinansiesDbContext(DbContextOptions<FinansiesDbContext> options)
        :DbContext(options) 
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetFilter> BudgetFilters { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }
        public DbSet<PlannedTransaction> PlannedTransactions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionTag> TransactionTags { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());

            modelBuilder.ApplyConfiguration(new BudgetConfiguration());

            modelBuilder.ApplyConfiguration(new BudgetFilterConfiguration());

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());

            modelBuilder.ApplyConfiguration(new DebtConfiguration());

            modelBuilder.ApplyConfiguration(new FamilyMemberConfiguration());

            modelBuilder.ApplyConfiguration(new PlannedTransactionConfiguration());

            modelBuilder.ApplyConfiguration(new TransactionConfiguration());

            modelBuilder.ApplyConfiguration(new TransactionTagConfiguration());

            modelBuilder.ApplyConfiguration(new TransferConfiguration());

            modelBuilder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
