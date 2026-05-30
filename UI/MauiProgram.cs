using BuisnessLogic.BudgetService;
using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.Repositories;
using BuisnessLogic.UseCases;
using CommunityToolkit.Maui;
using DataAccess;
using DataAccess.RepositoriesImplementation;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Maui.LifecycleEvents;
using SkiaSharp.Views.Maui.Controls.Hosting;
using UI.OrderingServices;
using UI.Popups;
using UI.PopUps.DependencyInjection;
using UI.Statistics;
using UI.ViewModels;
using UI.ViewModels.DependencyInjection;
using UI.Views.DependencyInjection;

#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif


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
                .UseSkiaSharp()
                .UseLiveCharts()
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
                .AddAnalytics()
                .AddOrderingServices()
                .AddViewModels()
                .AddViews()
                .AddPopUps();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
    events.AddWindows(w =>
    {
        w.OnWindowCreated(window =>
        {//If you need to completely hide the minimized maximized close button, you need to set this value to false.
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var _appWindow = AppWindow.GetFromWindowId(myWndId);
            _appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);

            switch (_appWindow.Presenter)
                    {
                        case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                            overlappedPresenter.Maximize();
                            break;
                    }
        });
    });
#endif
            });

            return builder.Build();
        }
    }
}
