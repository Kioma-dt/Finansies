using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Views;

namespace UI.ViewModels.DependencyInjection
{
    public static class DependencyInjectionViewModels
    {
        public static IServiceCollection AddViewModels(this
            IServiceCollection services)
        {
            services.AddSingleton<TransactionsViewModel>();
            services.AddSingleton<PlannedTransactionsViewModel>();
            services.AddSingleton<AccountViewModel>();
            services.AddSingleton<CategoryViewModel>();
            services.AddSingleton<FamilyMemberViewModel>();
            services.AddSingleton<BudgetViewModel>();
            services.AddSingleton<DebtViewModel>();
            services.AddSingleton<TransfersViewModel>();

            services.AddSingleton<DateRangeSelectorViewModel>();

            services.AddSingleton<StatisticsViewModel>();

            services.AddSingleton<MainPageViewModel>();

            services.AddSingleton<AuthentificationPageViewModel>();

            return services;
        }
    }
}
