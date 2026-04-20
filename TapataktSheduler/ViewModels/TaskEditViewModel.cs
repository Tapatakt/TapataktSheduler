using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана редактирования существующего дела.
/// Изменения названия, времени и привязок сохраняются автоматически.
/// </summary>
public sealed partial class TaskEditViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private readonly IDayTypeService _dayTypeService;
    private readonly ITaskTypeBindingService _taskTypeBindingService;
    private readonly ITaskDayBindingService _taskDayBindingService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    /// <summary>
    /// Название дела.
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// Время напоминания по умолчанию.
    /// </summary>
    [ObservableProperty]
    private TimeSpan _defaultReminderTime;

    /// <summary>
    /// Интервал повторных напоминаний в минутах.
    /// </summary>
    [ObservableProperty]
    private int _repeatFrequencyMinutes;

    /// <summary>
    /// Привязки дела к типам дней.
    /// </summary>
    public ObservableCollection<TaskTypeBindingItem> TypeBindings { get; } = new();

    /// <summary>
    /// Прямые привязки дела к датам.
    /// </summary>
    public ObservableCollection<TaskDayBindingItem> DayBindings { get; } = new();

    private Guid _taskId;
    private TimeSpan _lastDefaultReminderTime;
    private bool _isInitializing;

    /// <summary>
    /// Создаёт новый экземпляр ViewModel редактирования дела.
    /// </summary>
    /// <param name="taskService">Сервис дел.</param>
    /// <param name="dayTypeService">Сервис типов дней.</param>
    /// <param name="taskTypeBindingService">Сервис привязок к типам.</param>
    /// <param name="taskDayBindingService">Сервис привязок к датам.</param>
    /// <param name="navigationService">Сервис навигации.</param>
    /// <param name="dialogService">Сервис диалогов.</param>
    public TaskEditViewModel(
        ITaskService taskService,
        IDayTypeService dayTypeService,
        ITaskTypeBindingService taskTypeBindingService,
        ITaskDayBindingService taskDayBindingService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _taskService = taskService;
        _dayTypeService = dayTypeService;
        _taskTypeBindingService = taskTypeBindingService;
        _taskDayBindingService = taskDayBindingService;
        _navigationService = navigationService;
        _dialogService = dialogService;
        TypeBindings.CollectionChanged += OnTypeBindingsCollectionChanged;
        DayBindings.CollectionChanged += OnDayBindingsCollectionChanged;
    }

    /// <summary>
    /// Инициализирует ViewModel для указанного дела.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    public void Initialize(Guid taskId)
    {
        _isInitializing = true;
        _taskId = taskId;
        TypeBindings.Clear();
        DayBindings.Clear();
        LoadExistingTask(taskId);
        _isInitializing = false;
    }

    /// <summary>
    /// При изменении времени по умолчанию обновляет время в привязках,
    /// где оно совпадало с предыдущим значением по умолчанию.
    /// </summary>
    /// <param name="value">Новое время по умолчанию.</param>
    partial void OnDefaultReminderTimeChanged(TimeSpan value)
    {
        foreach (TaskTypeBindingItem item in TypeBindings)
            if (!item.IsCustomReminderTime)
                item.SetReminderTime(value);

        foreach (TaskDayBindingItem item in DayBindings)
            if (!item.IsCustomReminderTime)
                item.SetReminderTime(value);

        _lastDefaultReminderTime = value;
        AutoSave();
    }

    /// <summary>
    /// Загружает данные существующего дела и его привязки.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    private void LoadExistingTask(Guid taskId)
    {
        PlannedTask? task = _taskService.GetPlannedTask(taskId);
        if (task == null)
            return;

        Name = task.Name;
        DefaultReminderTime = task.DefaultReminderTime ?? TimeSpan.Zero;
        _lastDefaultReminderTime = DefaultReminderTime;
        RepeatFrequencyMinutes = (int)(task.RepeatFrequency?.TotalMinutes ?? 0);

        foreach (TaskTypeBinding binding in _taskTypeBindingService.GetTaskTypeBindings(taskId: taskId))
        {
            DayType? dayType = _dayTypeService.GetDayType(binding.DayTypeId);
            TaskTypeBindingItem item = new()
            {
                BindingId = binding.Id,
                DayTypeId = binding.DayTypeId,
                DayTypeName = dayType?.Name ?? "Неизвестный",
                IsCustomReminderTime = binding.ReminderTime != null
            };
            item.SetReminderTime(binding.ReminderTime ?? task.DefaultReminderTime ?? TimeSpan.Zero);
            TypeBindings.Add(item);
        }

        foreach (TaskDayBinding binding in _taskDayBindingService.GetTaskDayBindings(taskId: taskId))
        {
            TaskDayBindingItem item = new()
            {
                BindingId = binding.Id,
                Date = binding.Date,
                IsCustomReminderTime = binding.ReminderTime != null
            };
            item.SetReminderTime(binding.ReminderTime ?? task.DefaultReminderTime ?? TimeSpan.Zero);
            DayBindings.Add(item);
        }
    }

    /// <summary>
    /// Возвращает список типов дней, ещё не привязанных к этому делу.
    /// </summary>
    private List<DayType> GetAvailableDayTypes()
    {
        HashSet<Guid> boundTypeIds = TypeBindings.Select(tb => tb.DayTypeId).ToHashSet();
        return _dayTypeService.GetDayTypes()
            .Where(dt => !boundTypeIds.Contains(dt.Id))
            .OrderBy(dt => dt.Name)
            .ToList();
    }

    /// <summary>
    /// Добавляет привязку к типу дня через диалог выбора.
    /// </summary>
    [RelayCommand]
    private async Task AddTypeBindingAsync()
    {
        List<DayType> available = GetAvailableDayTypes();
        if (available.Count == 0)
            return;

        Guid? result = await _dialogService.ShowDayTypePickerAsync(available);
        if (!result.HasValue)
            return;

        DayType? dayType = available.FirstOrDefault(dt => dt.Id == result.Value);
        if (dayType == null)
            return;

        TaskTypeBindingItem newTypeItem = new()
        {
            BindingId = Guid.Empty,
            DayTypeId = dayType.Id,
            DayTypeName = dayType.Name
        };
        newTypeItem.SetReminderTime(DefaultReminderTime);
        TypeBindings.Add(newTypeItem);
    }

    /// <summary>
    /// Удаляет привязку к типу дня.
    /// </summary>
    /// <param name="item">Элемент привязки для удаления.</param>
    [RelayCommand]
    private void RemoveTypeBinding(TaskTypeBindingItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        TypeBindings.Remove(item);
    }

    /// <summary>
    /// Добавляет прямую привязку к выбранной дате через диалог.
    /// </summary>
    [RelayCommand]
    private async Task AddDayBindingAsync()
    {
        DateTime? result = await _dialogService.ShowDatePickerAsync(null);
        if (!result.HasValue)
            return;

        DateTime normalized = result.Value.Date;
        if (DayBindings.Any(b => b.Date == normalized))
            return;

        TaskDayBindingItem newDayItem = new()
        {
            BindingId = Guid.Empty,
            Date = normalized
        };
        newDayItem.SetReminderTime(DefaultReminderTime);
        DayBindings.Add(newDayItem);
    }

    /// <summary>
    /// Удаляет прямую привязку к дате.
    /// </summary>
    /// <param name="item">Элемент привязки для удаления.</param>
    [RelayCommand]
    private void RemoveDayBinding(TaskDayBindingItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        DayBindings.Remove(item);
    }

    /// <summary>
    /// Переходит на экран редактирования типа дня.
    /// </summary>
    /// <param name="dayTypeId">Идентификатор типа дня.</param>
    [RelayCommand]
    private Task EditDayTypeAsync(Guid dayTypeId) => _navigationService.GoToDayTypeEditAsync(dayTypeId);

    /// <summary>
    /// Переходит на экран дела выбранной даты.
    /// </summary>
    /// <param name="date">Дата.</param>
    [RelayCommand]
    private Task EditDayAsync(DateTime date) => _navigationService.GoToDayTasksAsync(date);

    /// <summary>
    /// Удаляет текущее дело с подтверждением.
    /// </summary>
    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirmed = await _dialogService.ShowConfirmationAsync(
            "Удаление",
            $"Удалить дело \"{Name}\"?");
        if (!confirmed)
            return;

        _taskService.DeletePlannedTask(_taskId);
        await _navigationService.GoBackAsync();
    }

    /// <summary>
    /// Сохраняет изменения дела и синхронизирует привязки.
    /// </summary>
    public void AutoSave()
    {
        if (_isInitializing)
            return;

        PlannedTask? task = _taskService.GetPlannedTask(_taskId);
        if (task == null)
            return;

        task.Name = Name;
        task.DefaultReminderTime = DefaultReminderTime == TimeSpan.Zero ? null : DefaultReminderTime;
        int minutes = Math.Clamp(RepeatFrequencyMinutes, 1, 99);
        task.RepeatFrequency = TimeSpan.FromMinutes(minutes);
        _taskService.SavePlannedTask(task);
        SynchronizeBindings(task.Id);
    }

    /// <summary>
    /// Обрабатывает изменение коллекции привязок к типам дней.
    /// </summary>
    private void OnTypeBindingsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (TaskTypeBindingItem item in e.OldItems)
                item.PropertyChanged -= OnBindingItemPropertyChanged;
        if (e.NewItems != null)
            foreach (TaskTypeBindingItem item in e.NewItems)
                item.PropertyChanged += OnBindingItemPropertyChanged;
        AutoSave();
    }

    /// <summary>
    /// Обрабатывает изменение коллекции привязок к датам.
    /// </summary>
    private void OnDayBindingsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
            foreach (TaskDayBindingItem item in e.OldItems)
                item.PropertyChanged -= OnBindingItemPropertyChanged;
        if (e.NewItems != null)
            foreach (TaskDayBindingItem item in e.NewItems)
                item.PropertyChanged += OnBindingItemPropertyChanged;
        AutoSave();
    }

    /// <summary>
    /// Сохраняет дело при изменении времени напоминания в привязке.
    /// </summary>
    private void OnBindingItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskTypeBindingItem.ReminderTime) ||
            e.PropertyName == nameof(TaskDayBindingItem.ReminderTime))
            AutoSave();
    }

    /// <summary>
    /// Синхронизирует привязки в хранилище с текущим состоянием коллекций.
    /// Удаляет старые записи и создаёт новые.
    /// </summary>
    /// <param name="taskId">Идентификатор сохранённого дела.</param>
    private void SynchronizeBindings(Guid taskId)
    {
        foreach (TaskTypeBinding old in _taskTypeBindingService.GetTaskTypeBindings(taskId: taskId))
            _taskTypeBindingService.DeleteTaskTypeBinding(old.Id);

        foreach (TaskDayBinding old in _taskDayBindingService.GetTaskDayBindings(taskId: taskId))
            _taskDayBindingService.DeleteTaskDayBinding(old.Id);

        foreach (TaskTypeBindingItem item in TypeBindings)
            _taskTypeBindingService.SaveTaskTypeBinding(new TaskTypeBinding
            {
                Id = item.BindingId == Guid.Empty ? Guid.NewGuid() : item.BindingId,
                TaskId = taskId,
                DayTypeId = item.DayTypeId,
                ReminderTime = item.IsCustomReminderTime ? item.ReminderTime : null
            });

        foreach (TaskDayBindingItem item in DayBindings)
            _taskDayBindingService.SaveTaskDayBinding(new TaskDayBinding
            {
                Id = item.BindingId == Guid.Empty ? Guid.NewGuid() : item.BindingId,
                TaskId = taskId,
                Date = item.Date,
                ReminderTime = item.IsCustomReminderTime ? item.ReminderTime : null
            });
    }
}
