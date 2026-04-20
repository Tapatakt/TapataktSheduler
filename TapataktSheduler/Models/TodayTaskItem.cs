using CommunityToolkit.Mvvm.ComponentModel;

namespace TapataktSheduler.Models;

/// <summary>
/// UI-модель дела на главном экране (сегодня) с чекбоксом и временем напоминания.
/// </summary>
public sealed partial class TodayTaskItem : ObservableObject
{
    /// <summary>
    /// Идентификатор дела.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Название дела.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Итоговое время напоминания для сегодняшнего дня.
    /// </summary>
    public TimeSpan? ReminderTime { get; set; }

    /// <summary>
    /// Признак выполнения дела сегодня.
    /// </summary>
    [ObservableProperty]
    private bool _isCompleted;

    /// <summary>
    /// Символ чекбокса для отображения в UI.
    /// </summary>
    [ObservableProperty]
    private string _checkboxSymbol = "\u2610";

    /// <summary>
    /// Обновляет символ чекбокса при изменении состояния выполнения.
    /// </summary>
    /// <param name="value">Новое значение выполнения.</param>
    partial void OnIsCompletedChanged(bool value)
    {
        CheckboxSymbol = value ? "\u2611" : "\u2610";
    }
}
