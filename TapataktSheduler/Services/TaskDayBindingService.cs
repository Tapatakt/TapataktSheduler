using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса прямых привязок дел к датам в памяти.
/// </summary>
public sealed class TaskDayBindingService : ITaskDayBindingService
{
    private readonly Lock _lock = new();
    private readonly List<TaskDayBinding> _dayBindings = [];

    /// <inheritdoc />
    public List<TaskDayBinding> GetTaskDayBindings(Guid? taskId = null, DateTime? date = null)
    {
        lock (_lock)
        {
            IEnumerable<TaskDayBinding> query = _dayBindings;
            if (taskId.HasValue)
                query = query.Where(b => b.TaskId == taskId.Value);
            if (date.HasValue)
                query = query.Where(b => b.Date.Date == date.Value.Date);
            return [.. query];
        }
    }

    /// <inheritdoc />
    public void SaveTaskDayBinding(TaskDayBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);
        binding.Date = binding.Date.Date;

        lock (_lock)
        {
            TaskDayBinding? existing = _dayBindings.FirstOrDefault(b => b.Id == binding.Id);
            if (existing != null)
                _dayBindings.Remove(existing);

            if (binding.Id == Guid.Empty)
                binding.Id = Guid.NewGuid();

            _dayBindings.Add(binding);
        }
    }

    /// <inheritdoc />
    public void DeleteTaskDayBinding(Guid id)
    {
        lock (_lock)
            _dayBindings.RemoveAll(b => b.Id == id);
    }

    /// <inheritdoc />
    public void DeleteByTaskId(Guid taskId)
    {
        lock (_lock)
            _dayBindings.RemoveAll(b => b.TaskId == taskId);
    }
}
