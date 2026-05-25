using BuisnessLogic.BudgetService;
using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases;
using CommunityToolkit.Maui;
using DataAccess;
using DataAccess.RepositoriesImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UI.Popups;

using UI.PopUps.DependencyInjection;
using UI.ViewModels;
using UI.Views.DependencyInjection;
using UI.ViewModels.DependencyInjection;

namespace UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContextFactory<FinansiesDbContext, FinansiesDbContextFactory>();

            builder.Services.AddSingleton<IBudgetSpecificationsCreator, BudgetAccountSpecificationsCreator>();
            builder.Services.AddSingleton<IBudgetSpecificationsCreator, BudgetCategorySpecificationsCreator>();
            builder.Services.AddSingleton<IBudgetSpecificationsCreator, BudgetFamilyMemberSpecificationsCreator>();
            builder.Services.AddSingleton<IBudgetSpecificationsCreator, BudgetTransactionTagSpecificationsCreator>();
            builder.Services.AddSingleton<IBudgetSpecificationsCreator, BudgetTransactionTypeSpecificationsCreator>();

            builder.Services.AddSingleton<IBudgetSpecificationsCreatorsProvider, BudgetSpecificationsCreatorsProvider>();
            builder.Services.AddSingleton<IBudgetSpecificationsExtender, BudgetSpecificationsExtender>();

            builder.Services.AddSingleton<IDebtInterestCalculator, ComplexInterestCalcilator>();
            builder.Services.AddSingleton<IDebtInterestCalculator, FixedInterestCalcilator>();
            builder.Services.AddSingleton<IDebtInterestCalculator, NoneInterestCalculator>();
            builder.Services.AddSingleton<IDebtInterestCalculator, SimpleInterestCalculator>();

            builder.Services.AddSingleton<IDebtInterestCalculatorProvider, DebtInterestCalculatorProvider>();

            builder.Services.AddSingleton<IUserContext, UserContext>();

            //builder.Services.AddTransient<IAccountRepository, AccountRepository>();
            //builder.Services.AddTransient<IBudgetRepository, BudgetRepository>();
            //builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
            //builder.Services.AddTransient<IDebtRepository, DebtRepository>();
            //builder.Services.AddTransient<IFamilyMemberRepository, FamilyMemberRepository>();
            //builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
            //builder.Services.AddTransient<IPlannedTransactionRepository, PlannedTransactionRepository>();
            //builder.Services.AddTransient<ITransactionTagRepository, TransactionTagRepository>();
            //builder.Services.AddTransient<ITransferRepository, TransferRepository>();
            //builder.Services.AddTransient<IUserRepository, UserRepository>();

            builder.Services.AddRepositories();

            builder.Services.AddUseCassess();

            builder.Services.AddViewModels();

            builder.Services.AddViews();

            builder.Services.AddPopUps();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
