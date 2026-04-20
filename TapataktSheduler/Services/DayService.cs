using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса дней в памяти.
/// </summary>
/// <param name="taskService">Сервис дел.</param>
/// <param name="taskTypeBindingService">Сервис привязок к типам.</param>
/// <param name="taskDayBindingService">Сервис привязок к датам.</param>
/// <param name="taskExceptionService">Сервис исключений.</param>
public sealed class DayService(
    ITaskService taskService,
    ITaskTypeBindingService taskTypeBindingService,
    ITaskDayBindingService taskDayBindingService,
    ITaskExceptionService taskExceptionService) : IDayService
{
    private readonly Lock _lock = new();
    private readonly List<Day> _days = [];
    private readonly ITaskService _taskService = taskService;
    private readonly ITaskTypeBindingService _taskTypeBindingService = taskTypeBindingService;
    private readonly ITaskDayBindingService _taskDayBindingService = taskDayBindingService;
    private readonly ITaskExceptionService _taskExceptionService = taskExceptionService;


    /// <inheritdoc />
    public Day? GetDay(DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
            return _days.FirstOrDefault(d => d.Date == normalized);
    }

    /// <inheritdoc />
    public Day GetOrCreateDay(DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
        {
            Day? existing = _days.FirstOrDefault(d => d.Date == normalized);
            if (existing != null)
                return existing;

            Day day = new() { Id = Guid.NewGuid(), Date = normalized };
            _days.Add(day);
            return day;
        }
    }

    /// <inheritdoc />
    public void SaveDay(Day day)
    {
        ArgumentNullException.ThrowIfNull(day);
        day.Date = day.Date.Date;

        lock (_lock)
        {
            Day? existing = _days.FirstOrDefault(d => d.Id == day.Id);
            if (existing != null)
                _days.Remove(existing);

            if (day.Id == Guid.Empty)
                day.Id = Guid.NewGuid();

            _days.Add(day);
        }
    }

    /// <inheritdoc />
    public Dictionary<DateTime, Day> GetDaysForRange(DateTime start, DateTime end)
    {
        DateTime normalizedStart = start.Date;
        DateTime normalizedEnd = end.Date;
        lock (_lock)
            return _days
                .Where(d => d.Date >= normalizedStart && d.Date <= normalizedEnd)
                .ToDictionary(d => d.Date);
    }

    /// <inheritdoc />
    public List<Day> GetDays()
    {
        lock (_lock)
            return [.. _days];
    }

    /// <inheritdoc />
    public List<PlannedTask> GetTasksForDay(DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
        {
            Day? day = _days.FirstOrDefault(d => d.Date == normalized);
            Guid? dayTypeId = day?.Type?.Id;

            HashSet<Guid> taskIds = [];

            foreach (TaskDayBinding b in _taskDayBindingService.GetTaskDayBindings(date: normalized))
                taskIds.Add(b.TaskId);

            if (dayTypeId.HasValue)
                foreach (TaskTypeBinding b in _taskTypeBindingService.GetTaskTypeBindings(dayTypeId: dayTypeId.Value))
                    taskIds.Add(b.TaskId);

            HashSet<Guid> excluded = [.. _taskExceptionService
                .GetTaskExceptions(date: normalized)
                .Select(e => e.TaskId)];

            return [.. _taskService
                .GetPlannedTasks()
                .Where(t => taskIds.Contains(t.Id) && !excluded.Contains(t.Id))];
        }
    }

    /// <inheritdoc />
    public TimeSpan? GetReminderTime(Guid taskId, DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
        {
            TaskDayBinding? dayBinding = _taskDayBindingService
                .GetTaskDayBindings(taskId: taskId, date: normalized)
                .FirstOrDefault();
            if (dayBinding?.ReminderTime != null)
                return dayBinding.ReminderTime;

            Day? day = _days.FirstOrDefault(d => d.Date == normalized);
            if (day?.Type != null)
            {
                TaskTypeBinding? typeBinding = _taskTypeBindingService
                    .GetTaskTypeBindings(taskId: taskId, dayTypeId: day.Type.Id)
                    .FirstOrDefault();
                if (typeBinding?.ReminderTime != null)
                    return typeBinding.ReminderTime;
            }

            return _taskService.GetPlannedTask(taskId)?.DefaultReminderTime;
        }
    }

    /// <inheritdoc />
    public void CancelTaskForDay(Guid taskId, DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
        {
            TaskDayBinding? direct = _taskDayBindingService
                .GetTaskDayBindings(taskId: taskId, date: normalized)
                .FirstOrDefault();
            if (direct != null)
            {
                _taskDayBindingService.DeleteTaskDayBinding(direct.Id);
                return;
            }

            _taskExceptionService.AddTaskException(taskId, normalized);
        }
    }
}
