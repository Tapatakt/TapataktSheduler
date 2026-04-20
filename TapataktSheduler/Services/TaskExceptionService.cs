using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса исключений в памяти.
/// </summary>
public sealed class TaskExceptionService : ITaskExceptionService
{
    private readonly Lock _lock = new();
    private readonly List<TaskException> _exceptions = [];

    /// <inheritdoc />
    public List<TaskException> GetTaskExceptions(Guid? taskId = null, DateTime? date = null)
    {
        lock (_lock)
        {
            IEnumerable<TaskException> query = _exceptions;
            if (taskId.HasValue)
                query = query.Where(e => e.TaskId == taskId.Value);
            if (date.HasValue)
                query = query.Where(e => e.Date.Date == date.Value.Date);
            return [.. query];
        }
    }

    /// <inheritdoc />
    public void AddTaskException(Guid taskId, DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
            if (!_exceptions.Any(e => e.TaskId == taskId && e.Date == normalized))
                _exceptions.Add(new TaskException { Id = Guid.NewGuid(), TaskId = taskId, Date = normalized });
    }

    /// <inheritdoc />
    public void RemoveTaskException(Guid taskId, DateTime date)
    {
        DateTime normalized = date.Date;
        lock (_lock)
            _exceptions.RemoveAll(e => e.TaskId == taskId && e.Date == normalized);
    }

    /// <inheritdoc />
    public void DeleteByTaskId(Guid taskId)
    {
        lock (_lock)
            _exceptions.RemoveAll(e => e.TaskId == taskId);
    }
}
