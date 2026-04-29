using BuisnessLogic.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DbProxy
{
    public class FinansiesDbContextSyncProxy(DbContextOptions<FinansiesDbContext> options)
        :FinansiesDbContext(options)
    {
        FinansiesDbContext dbContext = new FinansiesDbContext(options);

        static Semaphore semaphore = new(1, 1);

        public override DbSet<Account> Accounts 
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.Accounts;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.Accounts  = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<Budget> Budgets
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.Budgets;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.Budgets = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<BudgetFilter> BudgetFilters
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.BudgetFilters;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.BudgetFilters = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<Category> Categories
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.Categories;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.Categories = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<Debt> Debts
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.Debts;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.Debts = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<FamilyMember> FamilyMembers
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.FamilyMembers;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.FamilyMembers = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<PlannedTransaction> PlannedTransactions
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.PlannedTransactions;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.PlannedTransactions = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<Transaction> Transactions
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.Transactions;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.Transactions = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<TransactionTag> TransactionTags
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.TransactionTags;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.TransactionTags = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<Transfer> Transfers
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.Transfers;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.Transfers = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public override DbSet<User> Users
        {
            get
            {
                try
                {
                    semaphore.WaitOne();
                    return dbContext.Users;
                }
                finally
                {
                    semaphore.Release();
                }
            }
            set
            {
                try
                {
                    semaphore.WaitOne();
                    dbContext.Users = value;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

    }
}
