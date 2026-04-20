namespace TapataktSheduler.Models;

/// <summary>
/// Прямая связь дела с конкретной датой.
/// Имеет приоритет над <see cref="TaskTypeBinding"/>.
/// </summary>
public sealed class TaskDayBinding
{
    /// <summary>
    /// Уникальный идентификатор записи связи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор дела.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Дата, к которой привязано дело.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Время напоминания для конкретной даты (переопределяет <see cref="TaskTypeBinding.ReminderTime"/> и <see cref="PlannedTask.DefaultReminderTime"/>).
    /// </summary>
    public TimeSpan? ReminderTime { get; set; }
}
