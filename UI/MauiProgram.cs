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

            builder.Services.AddSingleton<IUserContext, UserContext>();


            builder.Services.AddRepositories()
                .AddUseCassess()
                .AddViewModels()
                .AddViews()
                .AddPopUps();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
