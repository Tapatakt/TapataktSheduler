namespace TapataktSheduler.Models;

/// <summary>
/// Связь дела с типом дня: на все дни данного типа распространяется это дело.
/// </summary>
public sealed class TaskTypeBinding
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
    /// Идентификатор типа дня.
    /// </summary>
    public Guid DayTypeId { get; set; }

    /// <summary>
    /// Время напоминания для данного типа дня (переопределяет <see cref="PlannedTask.DefaultReminderTime"/>).
    /// </summary>
    public TimeSpan? ReminderTime { get; set; }
}
