namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления запланированными делами.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Возвращает все запланированные дела.
    /// </summary>
    List<Models.PlannedTask> GetPlannedTasks();

    /// <summary>
    /// Возвращает дело по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор дела.</param>
    /// <returns>Найденное дело или null.</returns>
    Models.PlannedTask? GetPlannedTask(Guid id);

    /// <summary>
    /// Сохраняет дело (добавление или обновление).
    /// </summary>
    /// <param name="task">Дело для сохранения.</param>
    void SavePlannedTask(Models.PlannedTask task);

    /// <summary>
    /// Удаляет дело вместе со всеми его связями, исключениями и выполнениями.
    /// </summary>
    /// <param name="id">Идентификатор дела.</param>
    void DeletePlannedTask(Guid id);
}
