namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления днями (конкретными датами).
/// </summary>
public interface IDayService
{
    /// <summary>
    /// Возвращает день по дате или null, если день ещё не создан.
    /// </summary>
    /// <param name="date">Дата дня.</param>
    /// <returns>Найденный день или null.</returns>
    Models.Day? GetDay(DateTime date);

    /// <summary>
    /// Возвращает день по дате, создавая новый при необходимости.
    /// </summary>
    /// <param name="date">Дата дня.</param>
    /// <returns>Существующий или новый день.</returns>
    Models.Day GetOrCreateDay(DateTime date);

    /// <summary>
    /// Сохраняет день (добавление или обновление).
    /// </summary>
    /// <param name="day">День для сохранения.</param>
    void SaveDay(Models.Day day);

    /// <summary>
    /// Возвращает дни за указанный диапазон дат.
    /// </summary>
    /// <param name="start">Начальная дата диапазона.</param>
    /// <param name="end">Конечная дата диапазона.</param>
    /// <returns>Словарь дней по дате.</returns>
    Dictionary<DateTime, Models.Day> GetDaysForRange(DateTime start, DateTime end);

    /// <summary>
    /// Возвращает все созданные дни.
    /// </summary>
    List<Models.Day> GetDays();

    /// <summary>
    /// Возвращает дела, запланированные на указанную дату, с учётом типа дня,
    /// прямых привязок и исключений.
    /// </summary>
    /// <param name="date">Целевая дата.</param>
    /// <returns>Список дел на дату.</returns>
    List<Models.PlannedTask> GetTasksForDay(DateTime date);

    /// <summary>
    /// Определяет конечное время напоминания для дела на конкретную дату.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    /// <param name="date">Дата напоминания.</param>
    /// <returns>Время напоминания или null.</returns>
    TimeSpan? GetReminderTime(Guid taskId, DateTime date);

    /// <summary>
    /// Отменяет дело на указанный день.
    /// Если существует прямая привязка к дате — удаляет её.
    /// Иначе добавляет дату в исключения.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    /// <param name="date">Дата отмены.</param>
    void CancelTaskForDay(Guid taskId, DateTime date);
}
