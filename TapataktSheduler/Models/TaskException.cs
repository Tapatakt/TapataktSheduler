namespace TapataktSheduler.Models;

/// <summary>
/// Исключение: в указанную дату дело не должно появляться и напоминать,
/// даже если оно привязано к типу дня или к дате.
/// </summary>
public sealed class TaskException
{
    /// <summary>
    /// Уникальный идентификатор записи исключения.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор дела.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Дата, на которую распространяется исключение.
    /// </summary>
    public DateTime Date { get; set; }
}
