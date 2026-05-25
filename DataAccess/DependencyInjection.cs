using BuisnessLogic.Repositories;
using DataAccess.RepositoriesImplementation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class DependencyInjectionRepositories
    {
        public static IServiceCollection AddRepositories(this
            IServiceCollection services)
        {
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IBudgetRepository, BudgetRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IDebtRepository, DebtRepository>();
            services.AddTransient<IFamilyMemberRepository, FamilyMemberRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IPlannedTransactionRepository, PlannedTransactionRepository>();
            services.AddTransient<ITransactionTagRepository, TransactionTagRepository>();
            services.AddTransient<ITransferRepository, TransferRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}
