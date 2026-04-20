using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана списка дел на выбранный день.
/// Редактирование комментария и типа дня применяется автоматически.
/// </summary>
/// <param name="dayService">Сервис дней.</param>
/// <param name="dayTypeService">Сервис типов дней.</param>
/// <param name="taskService">Сервис дел.</param>
/// <param name="taskDayBindingService">Сервис привязок к датам.</param>
/// <param name="taskTypeBindingService">Сервис привязок к типам.</param>
/// <param name="taskExceptionService">Сервис исключений.</param>
/// <param name="navigationService">Сервис навигации.</param>
/// <param name="dialogService">Сервис диалогов.</param>
public sealed partial class DayTasksViewModel(
    IDayService dayService,
    IDayTypeService dayTypeService,
    ITaskService taskService,
    ITaskDayBindingService taskDayBindingService,
    ITaskTypeBindingService taskTypeBindingService,
    ITaskExceptionService taskExceptionService,
    INavigationService navigationService,
    IDialogService dialogService) : BaseViewModel
{
    private readonly IDayService _dayService = dayService;
    private readonly IDayTypeService _dayTypeService = dayTypeService;
    private readonly ITaskService _taskService = taskService;
    private readonly ITaskDayBindingService _taskDayBindingService = taskDayBindingService;
    private readonly ITaskTypeBindingService _taskTypeBindingService = taskTypeBindingService;
    private readonly ITaskExceptionService _taskExceptionService = taskExceptionService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IDialogService _dialogService = dialogService;

    /// <summary>
    /// Событие, возникающее после перезагрузки списков дел.
    /// Используется для уведомления UI о необходимости пересчёта размеров.
    /// </summary>
    public event Action? TasksReloaded;

    /// <summary>
    /// Дата отображаемого дня.
    /// </summary>
    [ObservableProperty]
    private DateTime _date;

    /// <summary>
    /// Пользовательский комментарий к дню.
    /// </summary>
    [ObservableProperty]
    private string _comment = string.Empty;

    /// <summary>
    /// Выбранный тип дня (null, если не указан).
    /// </summary>
    [ObservableProperty]
    private DayType? _selectedDayType;

    /// <summary>
    /// Доступные типы дней для выбора в Picker.
    /// </summary>
    public ObservableCollection<DayType> DayTypes { get; } = new();

    /// <summary>
    /// Дела, привязанные напрямую к этой дате, с временем напоминания.
    /// </summary>
    public ObservableCollection<TaskDisplayItem> DirectTasks { get; } = new();

    /// <summary>
    /// Дела, привязанные через тип дня, с временем напоминания.
    /// </summary>
    public ObservableCollection<TaskDisplayItem> TypeTasks { get; } = new();


    /// <summary>
    /// Инициализирует ViewModel для указанной даты.
    /// </summary>
    /// <param name="date">Целевая дата.</param>
    public void Initialize(DateTime date)
    {
        Date = date.Date;
        List<DayType> dayTypes = _dayTypeService.GetDayTypes().OrderBy(dt => dt.Name).ToList();

        DayTypes.Clear();
        foreach (DayType dayType in dayTypes)
            DayTypes.Add(dayType);

        Day day = _dayService.GetOrCreateDay(Date);
        Comment = day.Comment;
        SelectedDayType = day.Type != null
            ? dayTypes.FirstOrDefault(dt => dt.Id == day.Type.Id)
            : null;
        LoadTasks();
    }

    /// <summary>
    /// Перезагружает данные дня из хранилища.
    /// Используется при возврате на экран для актуализации списков.
    /// </summary>
    public void Reload()
    {
        List<DayType> dayTypes = _dayTypeService.GetDayTypes().OrderBy(dt => dt.Name).ToList();

        DayTypes.Clear();
        foreach (DayType dayType in dayTypes)
            DayTypes.Add(dayType);

        Day day = _dayService.GetOrCreateDay(Date);
        Comment = day.Comment;
        SelectedDayType = day.Type != null
            ? dayTypes.FirstOrDefault(dt => dt.Id == day.Type.Id)
            : null;
        LoadTasks();
    }

    /// <summary>
    /// Автоматически сохраняет комментарий и тип дня.
    /// </summary>
    public void AutoSave()
    {
        Day day = _dayService.GetOrCreateDay(Date);
        day.Comment = Comment;
        day.Type = SelectedDayType;
        _dayService.SaveDay(day);
    }

    /// <summary>
    /// При изменении типа дня автоматически перезагружает списки дел.
    /// Сохранение вызывается отдельно из code-behind при потере фокуса/выборе.
    /// </summary>
    /// <param name="value">Новый выбранный тип дня.</param>
    partial void OnSelectedDayTypeChanged(DayType? value) => LoadTasks();

    /// <summary>
    /// Загружает дела на текущую дату с учётом итогового времени напоминания.
    /// </summary>
    private void LoadTasks()
    {
        DirectTasks.Clear();
        foreach (TaskDayBinding binding in _taskDayBindingService.GetTaskDayBindings(date: Date)
            .OrderBy(b => b.ReminderTime ?? _taskService.GetPlannedTask(b.TaskId)?.DefaultReminderTime ?? TimeSpan.MaxValue))
        {
            PlannedTask? task = _taskService.GetPlannedTask(binding.TaskId);
            if (task == null)
                continue;

            TimeSpan? reminder = binding.ReminderTime ?? task.DefaultReminderTime;
            DirectTasks.Add(new TaskDisplayItem
            {
                TaskId = task.Id,
                Name = task.Name,
                ReminderTime = reminder
            });
        }

        TypeTasks.Clear();
        if (SelectedDayType != null)
        {
            foreach (TaskTypeBinding binding in _taskTypeBindingService.GetTaskTypeBindings(dayTypeId: SelectedDayType.Id)
                .OrderBy(b => b.ReminderTime ?? _taskService.GetPlannedTask(b.TaskId)?.DefaultReminderTime ?? TimeSpan.MaxValue))
            {
                PlannedTask? task = _taskService.GetPlannedTask(binding.TaskId);
                if (task == null)
                    continue;

                TimeSpan? reminder = binding.ReminderTime ?? task.DefaultReminderTime;
                TypeTasks.Add(new TaskDisplayItem
                {
                    TaskId = task.Id,
                    Name = task.Name,
                    ReminderTime = reminder
                });
            }
        }

        TasksReloaded?.Invoke();
    }

    /// <summary>
    /// Открепляет дело от конкретной даты с подтверждением.
    /// </summary>
    /// <param name="task">Дело для открепления.</param>
    [RelayCommand]
    private async Task DetachDirectTaskAsync(TaskDisplayItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        bool confirmed = await _dialogService.ShowConfirmationAsync(
            "Открепление",
            $"Убрать дело \"{task.Name}\" с этого дня?");
        if (!confirmed)
            return;

        TaskDayBinding? binding = _taskDayBindingService.GetTaskDayBindings(taskId: task.TaskId, date: Date).FirstOrDefault();
        if (binding != null)
            _taskDayBindingService.DeleteTaskDayBinding(binding.Id);

        LoadTasks();
    }

    /// <summary>
    /// Добавляет день в исключения для дела, привязанного через тип.
    /// </summary>
    /// <param name="task">Дело для исключения.</param>
    [RelayCommand]
    private async Task DetachTypeTaskAsync(TaskDisplayItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        bool confirmed = await _dialogService.ShowConfirmationAsync(
            "Открепление",
            $"Убрать дело \"{task.Name}\" с этого дня?");
        if (!confirmed)
            return;

        _taskExceptionService.AddTaskException(task.TaskId, Date);
        LoadTasks();
    }

    /// <summary>
    /// Переходит на экран редактирования выбранного дела.
    /// </summary>
    /// <param name="taskId">Идентификатор дела.</param>
    [RelayCommand]
    private Task EditTaskAsync(Guid taskId) => _navigationService.GoToTaskEditAsync(taskId: taskId);

    /// <summary>
    /// Открывает диалог выбора времени для прямой привязки дела к дню.
    /// </summary>
    /// <param name="task">Дело для изменения времени.</param>
    [RelayCommand]
    private async Task EditDirectTaskTimeAsync(TaskDisplayItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        TimeSpan? result = await _dialogService.ShowTimePickerAsync(task.ReminderTime);
        if (!result.HasValue)
            return;

        TaskDayBinding? binding = _taskDayBindingService.GetTaskDayBindings(taskId: task.TaskId, date: Date).FirstOrDefault();
        if (binding != null)
        {
            binding.ReminderTime = result.Value;
            _taskDayBindingService.SaveTaskDayBinding(binding);
            LoadTasks();
        }
    }

    /// <summary>
    /// Открывает диалог выбора времени для привязки дела через тип дня.
    /// </summary>
    /// <param name="task">Дело для изменения времени.</param>
    [RelayCommand]
    private async Task EditTypeTaskTimeAsync(TaskDisplayItem task)
    {
        ArgumentNullException.ThrowIfNull(task);

        TimeSpan? result = await _dialogService.ShowTimePickerAsync(task.ReminderTime);
        if (!result.HasValue)
            return;

        if (SelectedDayType == null)
            return;

        TaskTypeBinding? binding = _taskTypeBindingService.GetTaskTypeBindings(taskId: task.TaskId, dayTypeId: SelectedDayType.Id).FirstOrDefault();
        if (binding != null)
        {
            binding.ReminderTime = result.Value;
            _taskTypeBindingService.SaveTaskTypeBinding(binding);
            LoadTasks();
        }
    }

    /// <summary>
    /// Переходит на экран создания нового дела для текущей даты.
    /// </summary>
    [RelayCommand]
    private Task CreateTaskAsync() => _navigationService.GoToTaskCreateAsync(date: Date);

    /// <summary>
    /// Добавляет существующее дело к текущему дню через диалог выбора.
    /// </summary>
    [RelayCommand]
    private async Task AddExistingTaskAsync()
    {
        HashSet<Guid> attachedIds = DirectTasks.Select(t => t.TaskId).ToHashSet();
        List<PlannedTask> available = [.. _taskService.GetPlannedTasks()
            .Where(t => !attachedIds.Contains(t.Id))
            .OrderBy(t => t.Name)];

        if (available.Count == 0)
            return;

        Guid? result = await _dialogService.ShowTaskPickerAsync(available);
        if (!result.HasValue)
            return;

        _taskDayBindingService.SaveTaskDayBinding(new TaskDayBinding
        {
            Id = Guid.NewGuid(),
            TaskId = result.Value,
            Date = Date,
            ReminderTime = null
        });

        LoadTasks();
    }

    /// <summary>
    /// Сбрасывает выбранный тип дня (устанавливает в null).
    /// </summary>
    [RelayCommand]
    private void ClearDayType() => SelectedDayType = null;
}
