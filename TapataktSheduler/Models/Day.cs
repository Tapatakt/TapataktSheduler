namespace TapataktSheduler.Models;

/// <summary>
/// Представляет конкретный календарный день.
/// Содержит идентификатор, дату, опциональный комментарий и тип дня.
/// Список дел на этот день запрашивается через <see cref="Services.IDayService"/>.
/// </summary>
public sealed class Day
{
    /// <summary>
    /// Уникальный идентификатор дня.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Дата дня (без времени).
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Пользовательский комментарий к дню.
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Тип дня (может быть null, если не указан).
    /// </summary>
    public DayType? Type { get; set; }
}
