namespace TapataktSheduler.Models;

/// <summary>
/// UI-модель для отображения дела в списке с учётом времени напоминания.
/// </summary>
public sealed class TaskDisplayItem
{
    /// <summary>
    /// Идентификатор дела.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Название дела.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Итоговое время напоминания для отображаемой даты.
    /// </summary>
    public TimeSpan? ReminderTime { get; set; }
}
