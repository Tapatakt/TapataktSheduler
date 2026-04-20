namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления настройками приложения.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Возвращает текущие настройки приложения.
    /// </summary>
    Models.AppSettings GetSettings();

    /// <summary>
    /// Сохраняет настройки приложения.
    /// </summary>
    /// <param name="settings">Настройки для сохранения.</param>
    void SaveSettings(Models.AppSettings settings);
}
