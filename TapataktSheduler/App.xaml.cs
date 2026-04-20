using System.Globalization;

namespace TapataktSheduler;

/// <summary>
/// Корневой класс приложения.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Создаёт новый экземпляр приложения и устанавливает русскую локаль.
    /// </summary>
    public App()
    {
        CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
        CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Exception ex = e.ExceptionObject as Exception ?? new Exception("Unknown unhandled exception");
            System.Diagnostics.Debug.WriteLine($"UnhandledException: {ex}");
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            System.Diagnostics.Debug.WriteLine($"UnobservedTaskException: {e.Exception}");
            e.SetObserved();
        };

        InitializeComponent();
    }

    /// <summary>
    /// Создаёт главное окно приложения с корневым Shell.
    /// </summary>
    /// <param name="activationState">Состояние активации.</param>
    /// <returns>Главное окно приложения.</returns>
    protected override Window CreateWindow(IActivationState? activationState) => new(new AppShell());
}
