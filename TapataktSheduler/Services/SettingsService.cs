using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса настроек в памяти.
/// </summary>
public sealed class SettingsService : ISettingsService
{
    private readonly Lock _lock = new();
    private AppSettings _settings = new();

    /// <inheritdoc />
    public AppSettings GetSettings()
    {
        lock (_lock)
            return _settings;
    }

    /// <inheritdoc />
    public void SaveSettings(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        lock (_lock)
            _settings = settings;
    }
}
