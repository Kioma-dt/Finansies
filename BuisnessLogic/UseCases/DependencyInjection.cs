using BuisnessLogic.BudgetService;
using BuisnessLogic.DebtInterestCalculator;
using Microsoft.Extensions.DependencyInjection;

namespace BuisnessLogic.UseCases
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUseCassess(this
            IServiceCollection services)
        {
            services.AddSingleton<IBudgetSpecificationsCreator, BudgetAccountSpecificationsCreator>();
            services.AddSingleton<IBudgetSpecificationsCreator, BudgetCategorySpecificationsCreator>();
            services.AddSingleton<IBudgetSpecificationsCreator, BudgetFamilyMemberSpecificationsCreator>();
            services.AddSingleton<IBudgetSpecificationsCreator, BudgetTransactionTagSpecificationsCreator>();
            services.AddSingleton<IBudgetSpecificationsCreator, BudgetTransactionTypeSpecificationsCreator>();

            services.AddSingleton<IBudgetSpecificationsCreatorsProvider, BudgetSpecificationsCreatorsProvider>();
            services.AddSingleton<IBudgetSpecificationsExtender, BudgetSpecificationsExtender>();

            services.AddSingleton<IDebtInterestCalculator, ComplexInterestCalcilator>();
            services.AddSingleton<IDebtInterestCalculator, FixedInterestCalcilator>();
            services.AddSingleton<IDebtInterestCalculator, NoneInterestCalculator>();
            services.AddSingleton<IDebtInterestCalculator, SimpleInterestCalculator>();

            services.AddSingleton<IDebtInterestCalculatorProvider, DebtInterestCalculatorProvider>();

            services.AddMediatR(conf =>

                conf.RegisterServicesFromAssembly(typeof(DependencyInjection)
                    .Assembly));
            return services;
        }

    }
}
