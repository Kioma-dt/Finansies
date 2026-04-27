using UI.Views;
using UI.ViewModels;
using UI.Popups;
using BuisnessLogic.Repositories;
using BuisnessLogic.Services;
using CommunityToolkit.Maui;
using DataAccess;
using DataAccess.RepositoriesImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

            builder.Services.AddDbContext<FinansiesDbContext>(options =>
                options.UseMySql(
                 "server=localhost;database=finansies_db;user=root;password=Kioma220;",
                 new MySqlServerVersion(new Version(8, 0, 34))
             ));

            builder.Services.AddSingleton<IUserContext, UserContext>();

            builder.Services.AddSingleton<IAccountRepository, AccountRepository>();
            builder.Services.AddSingleton<IBudgetRepository, BudgetRepository>();
            builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
            builder.Services.AddSingleton<IDebtRepository, DebtRepository>();
            builder.Services.AddSingleton<IFamilyMemberRepository, FamilyMemberRepository>();
            builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
            builder.Services.AddSingleton<IPlannedTransactionRepository, PlannedTransactionRepository>();
            builder.Services.AddSingleton<ITransactionTagRepository, TransactionTagRepository>();
            builder.Services.AddSingleton<ITransferRepository, TransferRepository>();
            builder.Services.AddSingleton<IUserRepository, UserRepository>();

            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddSingleton<IBudgetService, BudgetService>();
            //builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddSingleton<IDebtService, DebtService>();
            //builder.Services.AddScoped<IFamilyMemberService, FamilyMemberService>();
            builder.Services.AddSingleton<ITransactionService, TransactionService>();
            builder.Services.AddSingleton<IPlannedTransactionService, PlannedTransactionService>();
            //builder.Services.AddScoped<ITransactionTagService, TransactionTagService>();
            builder.Services.AddSingleton<ITransferService, TransferService>();
            //builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<TransactionsViewModel>();
            builder.Services.AddSingleton<AccountViewModel>();

            builder.Services.AddSingleton<TransactionView>();
            builder.Services.AddSingleton<AccountView>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<AccountCreatePopUp>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
