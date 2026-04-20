namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления привязками дел к типам дней.
/// </summary>
public interface ITaskTypeBindingService
{
    /// <summary>
    /// Возвращает привязки дел к типам дней.
    /// </summary>
    /// <param name="taskId">Фильтр по делу (опционально).</param>
    /// <param name="dayTypeId">Фильтр по типу дня (опционально).</param>
    /// <returns>Список привязок.</returns>
    List<Models.TaskTypeBinding> GetTaskTypeBindings(Guid? taskId = null, Guid? dayTypeId = null);

    /// <summary>
    /// Сохраняет привязку дела к типу дня.
    /// </summary>
    /// <param name="binding">Привязка для сохранения.</param>
    void SaveTaskTypeBinding(Models.TaskTypeBinding binding);

    /// <summary>
    /// Удаляет привязку дела к типу дня.
    /// </summary>
    /// <param name="id">Идентификатор привязки.</param>
    void DeleteTaskTypeBinding(Guid id);

    /// <summary>
    /// Удаляет все привязки указанного дела к типам дней.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    void DeleteByTaskId(Guid taskId);

}
