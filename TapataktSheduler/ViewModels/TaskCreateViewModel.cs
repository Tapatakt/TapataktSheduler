using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана создания нового дела.
/// </summary>
public sealed partial class TaskCreateViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private readonly ITaskTypeBindingService _taskTypeBindingService;
    private readonly ITaskDayBindingService _taskDayBindingService;
    private readonly INavigationService _navigationService;

    private DateTime? _prefillDate;
    private Guid? _prefillDayTypeId;

    /// <summary>
    /// Название дела.
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// Время напоминания по умолчанию.
    /// </summary>
    [ObservableProperty]
    private TimeSpan _defaultReminderTime = TimeSpan.FromHours(19);

    /// <summary>
    /// Интервал повторных напоминаний в минутах.
    /// </summary>
    [ObservableProperty]
    private int _repeatFrequencyMinutes = 5;

    /// <summary>
    /// Создаёт новый экземпляр ViewModel.
    /// </summary>
    /// <param name="taskService">Сервис дел.</param>
    /// <param name="taskTypeBindingService">Сервис привязок к типам.</param>
    /// <param name="taskDayBindingService">Сервис привязок к датам.</param>
    /// <param name="navigationService">Сервис навигации.</param>
    public TaskCreateViewModel(
        ITaskService taskService,
        ITaskTypeBindingService taskTypeBindingService,
        ITaskDayBindingService taskDayBindingService,
        INavigationService navigationService)
    {
        _taskService = taskService;
        _taskTypeBindingService = taskTypeBindingService;
        _taskDayBindingService = taskDayBindingService;
        _navigationService = navigationService;
    }

    /// <summary>
    /// Инициализирует ViewModel с предзаполненными параметрами.
    /// </summary>
    /// <param name="prefillDate">Дата для автопривязки (опционально).</param>
    /// <param name="prefillDayTypeId">Тип дня для автопривязки (опционально).</param>
    public void Initialize(DateTime? prefillDate, Guid? prefillDayTypeId)
    {
        _prefillDate = prefillDate;
        _prefillDayTypeId = prefillDayTypeId;
    }

    /// <summary>
    /// Создаёт дело, добавляет автопривязку если есть параметры, и возвращается назад.
    /// </summary>
    [RelayCommand]
    private Task CreateAsync()
    {
        ArgumentException.ThrowIfNullOrEmpty(Name);

        PlannedTask task = BuildTask();
        _taskService.SavePlannedTask(task);
        CreateAutoBindings(task.Id);
        return _navigationService.GoBackAsync();
    }

    /// <summary>
    /// Создаёт дело, добавляет автопривязку если есть параметры, и подменяет экран на редактирование.
    /// </summary>
    [RelayCommand]
    private Task CreateAndEditAsync()
    {
        ArgumentException.ThrowIfNullOrEmpty(Name);

        PlannedTask task = BuildTask();
        _taskService.SavePlannedTask(task);
        CreateAutoBindings(task.Id);
        return _navigationService.ReplaceToTaskEditAsync(task.Id);
    }

    private PlannedTask BuildTask()
    {
        int minutes = Math.Clamp(RepeatFrequencyMinutes, 1, 99);
        return new PlannedTask
        {
            Name = Name,
            DefaultReminderTime = DefaultReminderTime == TimeSpan.Zero ? null : DefaultReminderTime,
            RepeatFrequency = TimeSpan.FromMinutes(minutes)
        };
    }

    private void CreateAutoBindings(Guid taskId)
    {
        if (_prefillDayTypeId.HasValue)
        {
            _taskTypeBindingService.SaveTaskTypeBinding(new TaskTypeBinding
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                DayTypeId = _prefillDayTypeId.Value,
                ReminderTime = null
            });
        }

        if (_prefillDate.HasValue)
        {
            _taskDayBindingService.SaveTaskDayBinding(new TaskDayBinding
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Date = _prefillDate.Value.Date,
                ReminderTime = null
            });
        }
    }
}
