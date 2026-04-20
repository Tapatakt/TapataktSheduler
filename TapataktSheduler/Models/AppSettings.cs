namespace TapataktSheduler.Models;

/// <summary>
/// Глобальные настройки приложения.
/// </summary>
public sealed class AppSettings
{
    /// <summary>
    /// Интервал повторных напоминаний по умолчанию.
    /// Используется, когда для конкретного дела частота не задана.
    /// </summary>
    public TimeSpan? DefaultRepeatFrequency { get; set; }

    /// <summary>
    /// Время ежедневного уведомления, которое просит указать тип сегодняшнего дня, если он ещё не установлен.
    /// </summary>
    public TimeSpan? DailyTypeRequestTime { get; set; }
}
