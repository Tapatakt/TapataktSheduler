namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления прямыми привязками дел к датам.
/// </summary>
public interface ITaskDayBindingService
{
    /// <summary>
    /// Возвращает прямые привязки дел к датам.
    /// </summary>
    /// <param name="taskId">Фильтр по делу (опционально).</param>
    /// <param name="date">Фильтр по дате (опционально).</param>
    /// <returns>Список привязок.</returns>
    List<Models.TaskDayBinding> GetTaskDayBindings(Guid? taskId = null, DateTime? date = null);

    /// <summary>
    /// Сохраняет прямую привязку дела к дате.
    /// </summary>
    /// <param name="binding">Привязка для сохранения.</param>
    void SaveTaskDayBinding(Models.TaskDayBinding binding);

    /// <summary>
    /// Удаляет прямую привязку дела к дате.
    /// </summary>
    /// <param name="id">Идентификатор привязки.</param>
    void DeleteTaskDayBinding(Guid id);

    /// <summary>
    /// Удаляет все прямые привязки указанного дела.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    void DeleteByTaskId(Guid taskId);
}
