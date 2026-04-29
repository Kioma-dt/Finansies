using UI.Views;
using UI.ViewModels;
using UI.Popups;
using BuisnessLogic.Repositories;
using BuisnessLogic.Services;
using DataAccess.DbProxy;
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

            builder.Services.AddTransient<IAccountRepository, AccountRepository>();
            builder.Services.AddTransient<IBudgetRepository, BudgetRepository>();
            builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
            builder.Services.AddTransient<IDebtRepository, DebtRepository>();
            builder.Services.AddTransient<IFamilyMemberRepository, FamilyMemberRepository>();
            builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
            builder.Services.AddTransient<IPlannedTransactionRepository, PlannedTransactionRepository>();
            builder.Services.AddTransient<ITransactionTagRepository, TransactionTagRepository>();
            builder.Services.AddTransient<ITransferRepository, TransferRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();

            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<IBudgetService, BudgetService>();
            //builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddTransient<IDebtService, DebtService>();
            //builder.Services.AddScoped<IFamilyMemberService, FamilyMemberService>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddTransient<IPlannedTransactionService, PlannedTransactionService>();
            //builder.Services.AddScoped<ITransactionTagService, TransactionTagService>();
            builder.Services.AddTransient<ITransferService, TransferService>();
            //builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddSingleton<TransactionsViewModel>();
            builder.Services.AddSingleton<AccountViewModel>();
            builder.Services.AddSingleton<CategoryViewModel>();
            builder.Services.AddSingleton<MainPageViewModel>();

            builder.Services.AddSingleton<TransactionView>();
            builder.Services.AddSingleton<AccountView>();
            builder.Services.AddSingleton<CategoryView>();
            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<AccountCreatePopUp>();
            builder.Services.AddTransient<CategoryCreatePopUp>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
