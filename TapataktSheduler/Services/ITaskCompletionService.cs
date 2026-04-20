namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления фактами выполнения дел.
/// </summary>
public interface ITaskCompletionService
{
    /// <summary>
    /// Возвращает записи о выполнении дел.
    /// </summary>
    /// <param name="taskId">Фильтр по делу (опционально).</param>
    /// <param name="date">Фильтр по дате (опционально).</param>
    /// <returns>Список выполнений.</returns>
    List<Models.TaskCompletion> GetTaskCompletions(Guid? taskId = null, DateTime? date = null);

    /// <summary>
    /// Отмечает дело выполненным в указанную дату.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    /// <param name="date">Дата выполнения.</param>
    void AddTaskCompletion(Guid taskId, DateTime date);

    /// <summary>
    /// Снимает отметку выполнения для дела в указанную дату.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    /// <param name="date">Дата выполнения.</param>
    void RemoveTaskCompletion(Guid taskId, DateTime date);

    /// <summary>
    /// Удаляет все выполнения указанного дела.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    void DeleteByTaskId(Guid taskId);
}
