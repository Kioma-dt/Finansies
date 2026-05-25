using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;
using UI.PopUps.ViewModels;
using UI.Views.Service;

namespace UI.Views.DependencyInjection
{
    public static class DependencyInjectionViews
    {
        public static IServiceCollection AddViews(this
            IServiceCollection services)
        {
            services.AddSingleton<TransactionView>();
            services.AddSingleton<PlannedTransactionView>();
            services.AddSingleton<AccountView>();
            services.AddSingleton<CategoryView>();
            services.AddSingleton<FamilyMemberView>();
            services.AddSingleton<BudgetView>();
            services.AddSingleton<DebtView>();
            services.AddSingleton<TransferView>();

            services.AddSingleton<DateRangeSelectorView>();

            services.AddSingleton<StatisticsView>();

            services.AddSingleton<MainPage>();

            services.AddSingleton<AuthentificationPage>();


            services.AddSingleton<IViewService, ViewService>();



            return services;
        }
    }
}
