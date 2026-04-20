namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления исключениями (отменёнными днями для дел).
/// </summary>
public interface ITaskExceptionService
{
    /// <summary>
    /// Возвращает дни-исключения для дел.
    /// </summary>
    /// <param name="taskId">Фильтр по делу (опционально).</param>
    /// <param name="date">Фильтр по дате (опционально).</param>
    /// <returns>Список исключений.</returns>
    List<Models.TaskException> GetTaskExceptions(Guid? taskId = null, DateTime? date = null);

    /// <summary>
    /// Добавляет исключение для дела на указанную дату.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    /// <param name="date">Дата исключения.</param>
    void AddTaskException(Guid taskId, DateTime date);

    /// <summary>
    /// Удаляет исключение для дела на указанную дату.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    /// <param name="date">Дата исключения.</param>
    void RemoveTaskException(Guid taskId, DateTime date);

    /// <summary>
    /// Удаляет все исключения указанного дела.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    void DeleteByTaskId(Guid taskId);
}
