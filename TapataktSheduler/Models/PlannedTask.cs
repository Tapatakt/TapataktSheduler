namespace TapataktSheduler.Models;

/// <summary>
/// Запланированное дело (задача) с настройками напоминаний.
/// Связи с типами дней, конкретными датами и исключениями хранятся
/// отдельно через соответствующие сервисы.
/// </summary>
public sealed class PlannedTask
{
    /// <summary>
    /// Уникальный идентификатор дела.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название дела.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Время напоминания по умолчанию.
    /// </summary>
    public TimeSpan? DefaultReminderTime { get; set; }

    /// <summary>
    /// Интервал повторных напоминаний, пока дело не отмечено выполненным.
    /// Если null — используется значение по умолчанию из настроек приложения.
    /// </summary>
    public TimeSpan? RepeatFrequency { get; set; }
}
