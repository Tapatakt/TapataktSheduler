namespace TapataktSheduler.Models;

/// <summary>
/// Факт выполнения дела в конкретную дату.
/// </summary>
public sealed class TaskCompletion
{
    /// <summary>
    /// Уникальный идентификатор записи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор выполненного дела.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Дата выполнения.
    /// </summary>
    public DateTime Date { get; set; }
}
