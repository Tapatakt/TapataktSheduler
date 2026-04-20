using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Сервис для отображения диалоговых сообщений пользователю.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Показывает всплывающее сообщение с одной кнопкой.
    /// </summary>
    /// <param name="title">Заголовок сообщения.</param>
    /// <param name="message">Текст сообщения.</param>
    /// <param name="cancel">Текст кнопки закрытия.</param>
    Task ShowAlertAsync(string title, string message, string cancel = "OK");

    /// <summary>
    /// Показывает диалог подтверждения с двумя кнопками.
    /// </summary>
    /// <param name="title">Заголовок диалога.</param>
    /// <param name="message">Текст вопроса.</param>
    /// <param name="accept">Текст кнопки подтверждения.</param>
    /// <param name="cancel">Текст кнопки отмены.</param>
    /// <returns>True, если пользователь нажал подтверждение; иначе False.</returns>
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Да", string cancel = "Нет");

    /// <summary>
    /// Показывает нативный диалог выбора времени.
    /// </summary>
    /// <param name="initialTime">Начальное время (null — текущее).</param>
    /// <returns>Выбранное время или null, если пользователь отменил.</returns>
    Task<TimeSpan?> ShowTimePickerAsync(TimeSpan? initialTime);

    /// <summary>
    /// Показывает нативный диалог выбора даты.
    /// </summary>
    /// <param name="initialDate">Начальная дата (null — сегодня).</param>
    /// <returns>Выбранная дата или null, если пользователь отменил.</returns>
    Task<DateTime?> ShowDatePickerAsync(DateTime? initialDate);

    /// <summary>
    /// Показывает нативный диалог выбора типа дня из списка.
    /// </summary>
    /// <param name="dayTypes">Список типов дней для выбора.</param>
    /// <returns>Идентификатор выбранного типа или null при отмене.</returns>
    Task<Guid?> ShowDayTypePickerAsync(List<DayType> dayTypes);

    /// <summary>
    /// Показывает нативный диалог выбора дела из списка.
    /// </summary>
    /// <param name="tasks">Список дел для выбора.</param>
    /// <returns>Идентификатор выбранного дела или null при отмене.</returns>
    Task<Guid?> ShowTaskPickerAsync(List<PlannedTask> tasks);
}
