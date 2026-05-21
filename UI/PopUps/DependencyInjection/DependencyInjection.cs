using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;
using UI.PopUps.ViewModels;

namespace UI.PopUps.DependencyInjection
{
    public static class DependencyInjectionPopUps
    {
        public static IServiceCollection AddPopUps(this
            IServiceCollection services)
        {
            services.AddTransient<AccountCreatePopUp>();
            services.AddTransient<CategoryCreatePopUp>();
            services.AddTransient<FamilyMemberCreatePopUp>();
            services.AddTransient<TransactionCreatePopUp>();
            services.AddTransient<PlannedTransactionCreatePopUp>();
            services.AddTransient<BudgetCreatePopUp>();
            services.AddTransient<DebtCreatePopUp>();
            services.AddTransient<TransferCreatePopUp>();

            services.AddTransient<DateRangePopUp>();

            services.AddTransient<AccountCreatePopUpModel>();
            services.AddTransient<BudgetCreatePopUpModel>();
            services.AddTransient<CategoryCreatePopUpModel>();
            services.AddTransient<TransferCreatePopUpModel>();

            return services;
        }
    }
}
