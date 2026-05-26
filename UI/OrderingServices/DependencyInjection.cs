using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Popups;
using UI.PopUps.Service;
using UI.PopUps.ViewModels;

namespace UI.OrderingServices
{
    public static class DependencyInjectionOrderingServices
    {
        public static IServiceCollection AddOrderingServices(this
            IServiceCollection services)
        {
            services.AddSingleton<ITransactionsOrderingServiceFactory, TransactionsOrderingServiceFactory>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByDescription>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByDate>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByAmount>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByType>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByAccountName>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByCategoryName>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByFamilyMemberName>();
            services.AddTransient<ITransactionsOrderingService, TransactionsOrderingServiceByDebtName>();

            services.AddSingleton<IPlannedTransactionsOrderingServiceFactory, PlannedTransactionsOrderingServiceFactory>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByDescription>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByStatus>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByDate>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByAmount>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByType>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByAccountName>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByCategoryName>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByFamilyMemberName>();
            services.AddTransient<IPlannedTransactionsOrderingService, PlannedTransactionsOrderingServiceByDebtName>();

            return services;
        }
    }
}
