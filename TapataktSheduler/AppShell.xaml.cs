using TapataktSheduler.Views;

namespace TapataktSheduler;

/// <summary>
/// Корневой Shell приложения, отвечающий за регистрацию маршрутов всех страниц.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// Создаёт новый экземпляр Shell и регистрирует маршруты.
    /// </summary>
    public AppShell()
    {
        InitializeComponent();

        // Регистрация маршрутов для навигации из ViewModel и сервисов.
        Routing.RegisterRoute("DayTasks", typeof(DayTasksPage));
        Routing.RegisterRoute("Calendar", typeof(CalendarPage));
        Routing.RegisterRoute("TaskCreate", typeof(TaskCreatePage));
        Routing.RegisterRoute("TaskEdit", typeof(TaskEditPage));
        Routing.RegisterRoute("Tasks", typeof(TasksPage));
        Routing.RegisterRoute("DayTypes", typeof(DayTypesPage));
        Routing.RegisterRoute("DayTypeCreate", typeof(DayTypeCreatePage));
        Routing.RegisterRoute("DayTypeEdit", typeof(DayTypeEditPage));
        Routing.RegisterRoute("Settings", typeof(SettingsPage));
    }
}
