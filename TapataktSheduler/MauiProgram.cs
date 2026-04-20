using Microsoft.Extensions.Logging;
using TapataktSheduler.Services;
using TapataktSheduler.ViewModels;
using TapataktSheduler.Views;

namespace TapataktSheduler;

/// <summary>
/// Точка входа конфигурации .NET MAUI приложения.
/// Регистрирует шрифты, сервисы, ViewModel и страницы.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Создаёт и настраивает экземпляр <see cref="MauiApp"/>.
    /// </summary>
    /// <returns>Сконфигурированное приложение.</returns>
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Регистрация сервисов приложения (singleton — глобальные состояния, transient — по запросу).
        builder.Services.AddSingleton<IDayTypeService, DayTypeService>();
        builder.Services.AddSingleton<IDayService, DayService>();
        builder.Services.AddSingleton<ITaskService, TaskService>();
        builder.Services.AddSingleton<ITaskTypeBindingService, TaskTypeBindingService>();
        builder.Services.AddSingleton<ITaskDayBindingService, TaskDayBindingService>();
        builder.Services.AddSingleton<ITaskExceptionService, TaskExceptionService>();
        builder.Services.AddSingleton<ITaskCompletionService, TaskCompletionService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();

        // Регистрация ViewModel.
        builder.Services.AddTransient<MainMenuViewModel>();
        builder.Services.AddTransient<DayTasksViewModel>();
        builder.Services.AddSingleton<CalendarViewModel>();
        builder.Services.AddTransient<TaskCreateViewModel>();
        builder.Services.AddTransient<TaskEditViewModel>();
        builder.Services.AddTransient<TasksViewModel>();
        builder.Services.AddTransient<DayTypesViewModel>();
        builder.Services.AddTransient<DayTypeCreateViewModel>();
        builder.Services.AddTransient<DayTypeEditViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        // Регистрация страниц для навигации и внедрения зависимостей.
        builder.Services.AddTransient<MainMenuPage>();
        builder.Services.AddTransient<DayTasksPage>();
        builder.Services.AddSingleton<CalendarPage>();
        builder.Services.AddTransient<TaskCreatePage>();
        builder.Services.AddTransient<TaskEditPage>();
        builder.Services.AddTransient<TasksPage>();
        builder.Services.AddTransient<DayTypesPage>();
        builder.Services.AddTransient<DayTypeCreatePage>();
        builder.Services.AddTransient<DayTypeEditPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        MauiApp app = builder.Build();

        // Разогрев страницы календаря: создаём Singleton-экземпляр заранее,
        // чтобы XAML-tree и данные были готовы до первого открытия.
        try
        {
            CalendarPage calendarPage = app.Services.GetRequiredService<CalendarPage>();
            if (calendarPage.BindingContext is CalendarViewModel calendarViewModel)
                calendarViewModel.Reload();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Calendar pre-warm failed: {ex}");
        }

        return app;
    }
}
