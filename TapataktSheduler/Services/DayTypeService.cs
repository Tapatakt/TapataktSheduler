using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса типов дней в памяти.
/// </summary>
/// <param name="dayService">Сервис дней.</param>
public sealed class DayTypeService(IDayService dayService) : IDayTypeService
{
    private readonly Lock _lock = new();
    private readonly List<DayType> _dayTypes = [];
    private readonly IDayService _dayService = dayService;


    /// <inheritdoc />
    public List<DayType> GetDayTypes()
    {
        lock (_lock)
            return [.. _dayTypes];
    }

    /// <inheritdoc />
    public DayType? GetDayType(Guid id)
    {
        lock (_lock)
            return _dayTypes.FirstOrDefault(dt => dt.Id == id);
    }

    /// <inheritdoc />
    public void SaveDayType(DayType dayType)
    {
        ArgumentNullException.ThrowIfNull(dayType);

        lock (_lock)
        {
            DayType? existing = _dayTypes.FirstOrDefault(dt => dt.Id == dayType.Id);
            if (existing != null)
            {
                existing.Name = dayType.Name;
                return;
            }

            if (dayType.Id == Guid.Empty)
                dayType.Id = Guid.NewGuid();

            _dayTypes.Add(dayType);
        }
    }

    /// <inheritdoc />
    public void DeleteDayType(Guid id)
    {
        lock (_lock)
        {
            DayType? existing = _dayTypes.FirstOrDefault(dt => dt.Id == id);
            if (existing != null)
                _dayTypes.Remove(existing);
        }
    }

    /// <inheritdoc />
    public bool CanDeleteDayType(Guid id)
    {
        DateTime today = DateTime.Today;
        List<Day> days = _dayService.GetDays();
        return !days.Any(d => d.Type?.Id == id && d.Date >= today);
    }
}
