namespace TapataktSheduler.Services;

/// <summary>
/// Сервис навигации между страницами приложения.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Переходит на главное меню.
    /// </summary>
    Task GoToMainMenuAsync();

    /// <summary>
    /// Переходит на экран календаря.
    /// </summary>
    Task GoToCalendarAsync();

    /// <summary>
    /// Переходит на экран дел указанного дня.
    /// </summary>
    /// <param name="date">Дата для отображения дел.</param>
    Task GoToDayTasksAsync(DateTime date);

    /// <summary>
    /// Переходит на экран создания нового дела.
    /// </summary>
    /// <param name="date">Предзаполненная дата (опционально).</param>
    /// <param name="dayTypeId">Предзаполненный тип дня (опционально).</param>
    Task GoToTaskCreateAsync(DateTime? date = null, Guid? dayTypeId = null);

    /// <summary>
    /// Переходит на экран редактирования дела.
    /// </summary>
    /// <param name="taskId">Идентификатор редактируемого дела.</param>
    Task GoToTaskEditAsync(Guid taskId);

    /// <summary>
    /// Подменяет текущий экран экраном редактирования дела.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    Task ReplaceToTaskEditAsync(Guid taskId);

    /// <summary>
    /// Переходит на экран списка всех дел.
    /// </summary>
    Task GoToTasksAsync();

    /// <summary>
    /// Переходит на экран списка типов дней.
    /// </summary>
    Task GoToDayTypesAsync();

    /// <summary>
    /// Переходит на экран создания нового типа дня.
    /// </summary>
    Task GoToDayTypeCreateAsync();

    /// <summary>
    /// Переходит на экран редактирования типа дня.
    /// </summary>
    /// <param name="dayTypeId">Идентификатор редактируемого типа.</param>
    Task GoToDayTypeEditAsync(Guid dayTypeId);

    /// <summary>
    /// Подменяет текущий экран экраном редактирования типа дня.
    /// </summary>
    /// <param name="dayTypeId">Идентификатор типа дня.</param>
    Task ReplaceToDayTypeEditAsync(Guid dayTypeId);

    /// <summary>
    /// Переходит на экран настроек.
    /// </summary>
    Task GoToSettingsAsync();

    /// <summary>
    /// Возвращает назад по стеку навигации.
    /// </summary>
    Task GoBackAsync();
}
