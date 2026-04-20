using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса запланированных дел в памяти.
/// </summary>
/// <param name="taskTypeBindingService">Сервис привязок к типам.</param>
/// <param name="taskDayBindingService">Сервис привязок к датам.</param>
/// <param name="taskExceptionService">Сервис исключений.</param>
/// <param name="taskCompletionService">Сервис выполнений.</param>
public sealed class TaskService(
    ITaskTypeBindingService taskTypeBindingService,
    ITaskDayBindingService taskDayBindingService,
    ITaskExceptionService taskExceptionService,
    ITaskCompletionService taskCompletionService) : ITaskService
{
    private readonly Lock _lock = new();
    private readonly List<PlannedTask> _tasks = [];
    private readonly ITaskTypeBindingService _taskTypeBindingService = taskTypeBindingService;
    private readonly ITaskDayBindingService _taskDayBindingService = taskDayBindingService;
    private readonly ITaskExceptionService _taskExceptionService = taskExceptionService;
    private readonly ITaskCompletionService _taskCompletionService = taskCompletionService;




    /// <inheritdoc />
    public List<PlannedTask> GetPlannedTasks()
    {
        lock (_lock)
            return [.. _tasks];
    }

    /// <inheritdoc />
    public PlannedTask? GetPlannedTask(Guid id)
    {
        lock (_lock)
            return _tasks.FirstOrDefault(t => t.Id == id);
    }

    /// <inheritdoc />
    public void SavePlannedTask(PlannedTask task)
    {
        ArgumentNullException.ThrowIfNull(task);

        lock (_lock)
        {
            PlannedTask? existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
                _tasks.Remove(existing);

            if (task.Id == Guid.Empty)
                task.Id = Guid.NewGuid();

            _tasks.Add(task);
        }
    }

    /// <inheritdoc />
    public void DeletePlannedTask(Guid id)
    {
        lock (_lock)
        {
            _taskTypeBindingService.DeleteByTaskId(id);
            _taskDayBindingService.DeleteByTaskId(id);
            _taskExceptionService.DeleteByTaskId(id);
            _taskCompletionService.DeleteByTaskId(id);
            _tasks.RemoveAll(t => t.Id == id);
        }
    }
}
