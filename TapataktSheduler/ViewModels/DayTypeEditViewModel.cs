using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана редактирования типа дня.
/// Изменения названия сохраняются автоматически.
/// </summary>
/// <remarks>
/// Создаёт новый экземпляр ViewModel.
/// </remarks>
/// <param name="dayTypeService">Сервис типов дней.</param>
/// <param name="taskTypeBindingService">Сервис привязок к типам.</param>
/// <param name="taskService">Сервис дел.</param>
/// <param name="navigationService">Сервис навигации.</param>
/// <param name="dialogService">Сервис диалогов.</param>
public sealed partial class DayTypeEditViewModel(
    IDayTypeService dayTypeService,
    ITaskTypeBindingService taskTypeBindingService,
    ITaskService taskService,
    INavigationService navigationService,
    IDialogService dialogService) : BaseViewModel
{
    private readonly IDayTypeService _dayTypeService = dayTypeService;
    private readonly ITaskTypeBindingService _taskTypeBindingService = taskTypeBindingService;
    private readonly ITaskService _taskService = taskService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IDialogService _dialogService = dialogService;

    /// <summary>
    /// Название типа дня.
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// Флаг возможности удаления текущего типа дня.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private bool _canDelete;

    /// <summary>
    /// Привязанные дела с временем напоминания.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<TaskDisplayItem> _attachedTasks = new();

    private Guid? _dayTypeId;


    /// <summary>
    /// Инициализирует ViewModel для указанного типа дня.
    /// </summary>
    /// <param name="dayTypeId">Идентификатор типа дня.</param>
    public void Initialize(Guid dayTypeId)
    {
        _dayTypeId = dayTypeId;
        AttachedTasks.Clear();

        DayType? dayType = _dayTypeService.GetDayType(dayTypeId);
        if (dayType == null)
        {
            Name = string.Empty;
            CanDelete = false;
            return;
        }

        Name = dayType.Name;
        CanDelete = _dayTypeService.CanDeleteDayType(dayTypeId);
        LoadTasks();
    }

    /// <summary>
    /// Перезагружает данные типа дня из хранилища.
    /// </summary>
    public void Reload()
    {
        if (!_dayTypeId.HasValue)
            return;

        DayType? dayType = _dayTypeService.GetDayType(_dayTypeId.Value);
        if (dayType == null)
            return;

        Name = dayType.Name;
        CanDelete = _dayTypeService.CanDeleteDayType(_dayTypeId.Value);
        LoadTasks();
    }

    /// <summary>
    /// Сохраняет изменения названия типа дня.
    /// </summary>
    public void AutoSave()
    {
        if (!_dayTypeId.HasValue)
            return;

        DayType? dayType = _dayTypeService.GetDayType(_dayTypeId.Value);
        if (dayType == null)
            return;

        dayType.Name = Name;
        _dayTypeService.SaveDayType(dayType);
    }

    /// <summary>
    /// Загружает списки привязанных и доступных дел.
    /// </summary>
    private void LoadTasks()
    {
        if (!_dayTypeId.HasValue)
            return;

        List<TaskDisplayItem> attachedItems = [];
        foreach (TaskTypeBinding binding in _taskTypeBindingService.GetTaskTypeBindings(dayTypeId: _dayTypeId.Value)
            .OrderBy(b => b.ReminderTime ?? _taskService.GetPlannedTask(b.TaskId)?.DefaultReminderTime ?? TimeSpan.MaxValue))
        {
            PlannedTask? task = _taskService.GetPlannedTask(binding.TaskId);
            if (task == null)
                continue;

            TimeSpan? reminder = binding.ReminderTime ?? task.DefaultReminderTime;
            attachedItems.Add(new TaskDisplayItem
            {
                TaskId = task.Id,
                Name = task.Name,
                ReminderTime = reminder
            });
        }

        AttachedTasks = new ObservableCollection<TaskDisplayItem>(attachedItems);
    }

    /// <summary>
    /// Удаляет текущий тип дня с подтверждением.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (!_dayTypeId.HasValue)
            return;

        bool confirmed = await _dialogService.ShowConfirmationAsync(
            "Удаление",
            $"Удалить тип дня \"{Name}\"?");
        if (!confirmed)
            return;

        _dayTypeService.DeleteDayType(_dayTypeId.Value);
        await _navigationService.GoBackAsync();
    }

    /// <summary>
    /// Переходит на экран редактирования выбранного дела.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    [RelayCommand]
    private Task EditTaskAsync(Guid taskId) => _navigationService.GoToTaskEditAsync(taskId: taskId);

    /// <summary>
    /// Открывает диалог выбора времени для привязанного дела.
    /// </summary>
    /// <param name="task">Дело для изменения времени.</param>
    [RelayCommand]
    private async Task EditTaskTimeAsync(TaskDisplayItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        if (!_dayTypeId.HasValue)
            return;

        TimeSpan? result = await _dialogService.ShowTimePickerAsync(task.ReminderTime);
        if (!result.HasValue)
            return;

        TaskTypeBinding? binding = _taskTypeBindingService.GetTaskTypeBindings(taskId: task.TaskId, dayTypeId: _dayTypeId.Value).FirstOrDefault();
        if (binding != null)
        {
            binding.ReminderTime = result.Value;
            _taskTypeBindingService.SaveTaskTypeBinding(binding);
            LoadTasks();
        }
    }

    /// <summary>
    /// Переходит на экран создания нового дела с привязкой к текущему типу.
    /// </summary>
    [RelayCommand]
    private Task CreateNewTaskAsync()
    {
        if (!_dayTypeId.HasValue)
            return Task.CompletedTask;

        return _navigationService.GoToTaskCreateAsync(dayTypeId: _dayTypeId.Value);
    }

    /// <summary>
    /// Привязывает существующее дело к текущему типу дня через диалог выбора.
    /// </summary>
    [RelayCommand]
    private async Task AddExistingTaskAsync()
    {
        if (!_dayTypeId.HasValue)
            return;

        HashSet<Guid> attachedIds = AttachedTasks.Select(a => a.TaskId).ToHashSet();
        List<PlannedTask> available = [.. _taskService.GetPlannedTasks()
            .Where(t => !attachedIds.Contains(t.Id))
            .OrderBy(t => t.Name)];

        if (available.Count == 0)
            return;

        Guid? result = await _dialogService.ShowTaskPickerAsync(available);
        if (!result.HasValue)
            return;

        _taskTypeBindingService.SaveTaskTypeBinding(new TaskTypeBinding
        {
            Id = Guid.NewGuid(),
            TaskId = result.Value,
            DayTypeId = _dayTypeId.Value,
            ReminderTime = null
        });

        LoadTasks();
    }

    /// <summary>
    /// Открепляет дело от текущего типа дня с подтверждением.
    /// </summary>
    /// <param name="task">Дело для открепления.</param>
    [RelayCommand]
    private async Task DetachTaskAsync(TaskDisplayItem task)
    {
        ArgumentNullException.ThrowIfNull(task);
        if (!_dayTypeId.HasValue)
            return;

        bool confirmed = await _dialogService.ShowConfirmationAsync(
            "Открепление",
            $"Открепить дело \"{task.Name}\" от типа дня?");
        if (!confirmed)
            return;

        TaskTypeBinding? binding = _taskTypeBindingService.GetTaskTypeBindings(taskId: task.TaskId, dayTypeId: _dayTypeId.Value).FirstOrDefault();
        if (binding != null)
            _taskTypeBindingService.DeleteTaskTypeBinding(binding.Id);

        LoadTasks();
    }
}
