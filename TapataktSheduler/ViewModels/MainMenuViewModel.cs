using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel главного меню приложения.
/// Отвечает за навигацию к основным разделам и отображение дел на сегодня.
/// </summary>
/// <param name="dayService">Сервис дней.</param>
/// <param name="taskCompletionService">Сервис выполнений.</param>
/// <param name="navigationService">Сервис навигации.</param>
/// <param name="dialogService">Сервис диалогов.</param>
public sealed partial class MainMenuViewModel(IDayService dayService, ITaskCompletionService taskCompletionService, INavigationService navigationService, IDialogService dialogService) : ObservableObject
{
    private readonly IDayService _dayService = dayService;
    private readonly ITaskCompletionService _taskCompletionService = taskCompletionService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IDialogService _dialogService = dialogService;

    /// <summary>
    /// Список дел на сегодня с возможностью отметки выполнения.
    /// </summary>
    public ObservableCollection<TodayTaskItem> TodayTasks { get; } = new();

    /// <summary>
    /// Комментарий к сегодняшнему дню.
    /// </summary>
    [ObservableProperty]
    private string _todayComment = string.Empty;


    /// <summary>
    /// Загружает дела на сегодня из хранилища с учётом выполнений.
    /// Невыполненные отображаются первыми, затем выполненные;
    /// внутри группы сортировка по времени напоминания.
    /// </summary>
    public void LoadTodayTasks()
    {
        DateTime today = DateTime.Today;
        TodayTasks.Clear();

        Day day = _dayService.GetOrCreateDay(today);
        TodayComment = day.Comment;

        List<PlannedTask> tasks = _dayService.GetTasksForDay(today);
        List<TaskCompletion> completions = _taskCompletionService.GetTaskCompletions(date: today);
        HashSet<Guid> completedIds = completions.Select(c => c.TaskId).ToHashSet();

        List<TodayTaskItem> items = [];
        foreach (PlannedTask task in tasks)
        {
            TimeSpan? reminder = _dayService.GetReminderTime(task.Id, today);
            bool isCompleted = completedIds.Contains(task.Id);
            items.Add(new TodayTaskItem
            {
                TaskId = task.Id,
                Name = task.Name,
                ReminderTime = reminder,
                IsCompleted = isCompleted
            });
        }

        items = [.. items.OrderBy(i => i.IsCompleted).ThenBy(i => i.ReminderTime ?? TimeSpan.MaxValue)];

        foreach (TodayTaskItem item in items)
            TodayTasks.Add(item);
    }

    /// <summary>
    /// Переключает состояние выполнения дела на сегодня.
    /// </summary>
    /// <param name="item">Дело для переключения.</param>
    [RelayCommand]
    private void ToggleTaskCompletion(TodayTaskItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        DateTime today = DateTime.Today;
        if (item.IsCompleted)
            _taskCompletionService.RemoveTaskCompletion(item.TaskId, today);
        else
            _taskCompletionService.AddTaskCompletion(item.TaskId, today);

        LoadTodayTasks();
    }

    /// <summary>
    /// Открывает редактор выбранного дела.
    /// </summary>
    /// <param name="item">Дело для редактирования.</param>
    [RelayCommand]
    private Task OpenTaskEditAsync(TodayTaskItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _navigationService.GoToTaskEditAsync(taskId: item.TaskId);
    }

    /// <summary>
    /// Переходит на экран календаря.
    /// </summary>
    [RelayCommand]
    private Task GoToCalendarAsync() => _navigationService.GoToCalendarAsync();

    /// <summary>
    /// Переходит на экран типов дней.
    /// </summary>
    [RelayCommand]
    private Task GoToDayTypesAsync() => _navigationService.GoToDayTypesAsync();

    /// <summary>
    /// Переходит на экран списка дел.
    /// </summary>
    [RelayCommand]
    private Task GoToTasksAsync() => _navigationService.GoToTasksAsync();

    /// <summary>
    /// Переходит на экран настроек.
    /// </summary>
    [RelayCommand]
    private Task GoToSettingsAsync() => _navigationService.GoToSettingsAsync();
}
