using Microsoft.EntityFrameworkCore;
using BuisnessLogic.Entities;
using DataAccess.Configurations;

namespace DataAccess
{
    public class FinansiesDbContext(DbContextOptions<FinansiesDbContext> options)
        :DbContext(options) 
    {
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Budget> Budgets { get; set; }
        public virtual DbSet<BudgetFilter> BudgetFilters { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Debt> Debts { get; set; }
        public virtual DbSet<FamilyMember> FamilyMembers { get; set; }
        public virtual DbSet<PlannedTransaction> PlannedTransactions { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<TransactionTag> TransactionTags { get; set; }
        public virtual DbSet<Transfer> Transfers { get; set; }
        public virtual DbSet<User> Users { get; set; }

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
