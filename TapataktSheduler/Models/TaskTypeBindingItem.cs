using CommunityToolkit.Mvvm.ComponentModel;

namespace TapataktSheduler.Models;

/// <summary>
/// UI-обёртка привязки дела к типу дня для экрана редактирования.
/// </summary>
public sealed partial class TaskTypeBindingItem : ObservableObject
{
    /// <summary>
    /// Идентификатор записи привязки (Guid.Empty для новых).
    /// </summary>
    public Guid BindingId { get; set; }

    /// <summary>
    /// Идентификатор типа дня.
    /// </summary>
    public Guid DayTypeId { get; set; }

    /// <summary>
    /// Название типа дня для отображения.
    /// </summary>
    public string DayTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Время напоминания для данного типа.
    /// </summary>
    [ObservableProperty]
    private TimeSpan _reminderTime;

    /// <summary>
    /// Флаг, указывающий, что время установлено пользователем явно.
    /// Если false — используется время по умолчанию дела.
    /// </summary>
    [ObservableProperty]
    private bool _isCustomReminderTime;

    private bool _isProgrammaticUpdate;

    /// <summary>
    /// Вызывается при изменении времени напоминания.
    /// Если изменение инициировано пользователем, помечает время как кастомное.
    /// </summary>
    /// <param name="value">Новое значение времени.</param>
    partial void OnReminderTimeChanged(TimeSpan value)
    {
        if (!_isProgrammaticUpdate)
            IsCustomReminderTime = true;
    }

    /// <summary>
    /// Устанавливает время напоминания программно, не помечая его как кастомное.
    /// </summary>
    /// <param name="value">Новое значение времени.</param>
    public void SetReminderTime(TimeSpan value)
    {
        _isProgrammaticUpdate = true;
        ReminderTime = value;
        _isProgrammaticUpdate = false;
    }
}
