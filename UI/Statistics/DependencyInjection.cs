using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.ViewModels;

namespace UI.Statistics
{
    public static class DependencyInjectionAnalytics
    {
        public static IServiceCollection AddAnalytics(this
            IServiceCollection services)
        {
            services.AddSingleton<IAnalyticsService, AnalyticsService>();

            return services;
        }
    }
}
