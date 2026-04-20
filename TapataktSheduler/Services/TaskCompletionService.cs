using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса фактов выполнения в памяти.
/// </summary>
public sealed class TaskCompletionService : ITaskCompletionService
{
    private readonly Lock _lock = new();
    private readonly List<TaskCompletion> _completions = [];

    /// <inheritdoc />
    public List<TaskCompletion> GetTaskCompletions(Guid? taskId = null, DateTime? date = null)
    {
        lock (_lock)
        {
            IEnumerable<TaskCompletion> query = _completions;
            if (taskId.HasValue)
                query = query.Where(c => c.TaskId == taskId.Value);
            if (date.HasValue)
                query = query.Where(c => c.Date.Date == date.Value.Date);
            return [.. query];
        }
    }

    /// <inheritdoc />
    public void AddTaskCompletion(Guid taskId, DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
            if (!_completions.Any(c => c.TaskId == taskId && c.Date == normalized))
                _completions.Add(new TaskCompletion { Id = Guid.NewGuid(), TaskId = taskId, Date = normalized });
    }

    /// <inheritdoc />
    public void RemoveTaskCompletion(Guid taskId, DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
            _completions.RemoveAll(c => c.TaskId == taskId && c.Date == normalized);
    }

    /// <inheritdoc />
    public void DeleteByTaskId(Guid taskId)
    {
        lock (_lock)
            _completions.RemoveAll(c => c.TaskId == taskId);
    }
}
