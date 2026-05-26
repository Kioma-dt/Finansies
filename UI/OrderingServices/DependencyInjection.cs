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

            return services;
        }
    }
}
