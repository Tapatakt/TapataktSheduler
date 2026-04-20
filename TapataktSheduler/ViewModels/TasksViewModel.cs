using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана списка всех дел.
/// Управляет загрузкой, добавлением и переходом к редактированию дел.
/// </summary>
public sealed partial class TasksViewModel : BaseViewModel
{
    private readonly ITaskService _taskService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Коллекция дел для отображения в списке.
    /// </summary>
    public ObservableCollection<PlannedTask> Tasks { get; } = new();

    /// <summary>
    /// Создаёт новый экземпляр ViewModel списка дел.
    /// </summary>
    /// <param name="taskService">Сервис задач.</param>
    /// <param name="navigationService">Сервис навигации.</param>
    public TasksViewModel(ITaskService taskService, INavigationService navigationService)
    {
        _taskService = taskService;
        _navigationService = navigationService;
        LoadTasks();
    }

    /// <summary>
    /// Перезагружает список дел из хранилища.
    /// </summary>
    [RelayCommand]
    public void LoadTasks()
    {
        Tasks.Clear();
        foreach (PlannedTask task in _taskService.GetPlannedTasks().OrderBy(t => t.Name))
            Tasks.Add(task);
    }

    /// <summary>
    /// Переходит на экран создания нового дела.
    /// </summary>
    [RelayCommand]
    private Task AddTaskAsync() => _navigationService.GoToTaskCreateAsync();

    /// <summary>
    /// Переходит на экран редактирования выбранного дела.
    /// </summary>
    /// <param name="task">Дело для редактирования.</param>
    [RelayCommand]
    private Task EditTaskAsync(PlannedTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        return _navigationService.GoToTaskEditAsync(taskId: task.Id);
    }
}
