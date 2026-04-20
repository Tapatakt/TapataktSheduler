using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса привязок дел к типам дней в памяти.
/// </summary>
public sealed class TaskTypeBindingService : ITaskTypeBindingService
{
    private readonly Lock _lock = new();
    private readonly List<TaskTypeBinding> _typeBindings = [];

    /// <inheritdoc />
    public List<TaskTypeBinding> GetTaskTypeBindings(Guid? taskId = null, Guid? dayTypeId = null)
    {
        lock (_lock)
        {
            IEnumerable<TaskTypeBinding> query = _typeBindings;
            if (taskId.HasValue)
                query = query.Where(b => b.TaskId == taskId.Value);
            if (dayTypeId.HasValue)
                query = query.Where(b => b.DayTypeId == dayTypeId.Value);
            return [.. query];
        }
    }

    /// <inheritdoc />
    public void SaveTaskTypeBinding(TaskTypeBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        lock (_lock)
        {
            TaskTypeBinding? existing = _typeBindings.FirstOrDefault(b => b.Id == binding.Id);
            if (existing != null)
                _typeBindings.Remove(existing);

            if (binding.Id == Guid.Empty)
                binding.Id = Guid.NewGuid();

            _typeBindings.Add(binding);
        }
    }

    /// <inheritdoc />
    public void DeleteTaskTypeBinding(Guid id)
    {
        lock (_lock)
            _typeBindings.RemoveAll(b => b.Id == id);
    }

    /// <inheritdoc />
    public void DeleteByTaskId(Guid taskId)
    {
        lock (_lock)
            _typeBindings.RemoveAll(b => b.TaskId == taskId);
    }
}
