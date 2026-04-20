using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана календаря для выбора дня.
/// Генерирует сетку дней текущего месяца с учётом типов дней.
/// </summary>
public sealed partial class CalendarViewModel : BaseViewModel
{
    private readonly IDayService _dayService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Текущий отображаемый месяц (первое число месяца).
    /// </summary>
    [ObservableProperty]
    private DateTime _currentMonth;

    /// <summary>
    /// Коллекция дней для отображения в сетке календаря.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<CalendarDay> _days = new();

    /// <summary>
    /// Создаёт новый экземпляр ViewModel календаря.
    /// </summary>
    /// <param name="dayService">Сервис дней.</param>
    /// <param name="navigationService">Сервис навигации.</param>
    public CalendarViewModel(IDayService dayService, INavigationService navigationService)
    {
        _dayService = dayService;
        _navigationService = navigationService;
        CurrentMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    }

    /// <inheritdoc cref="OnCurrentMonthChanged(DateTime)"/>
    partial void OnCurrentMonthChanged(DateTime value) => GenerateCalendar();

    /// <summary>
    /// Переключает календарь на предыдущий месяц.
    /// </summary>
    [RelayCommand]
    private void PreviousMonth() => CurrentMonth = CurrentMonth.AddMonths(-1);

    /// <summary>
    /// Переключает календарь на следующий месяц.
    /// </summary>
    [RelayCommand]
    private void NextMonth() => CurrentMonth = CurrentMonth.AddMonths(1);

    /// <summary>
    /// Переходит на экран дел выбранного дня.
    /// </summary>
    /// <param name="day">Выбранный день из сетки календаря.</param>
    [RelayCommand]
    private Task SelectDayAsync(CalendarDay day)
    {
        ArgumentNullException.ThrowIfNull(day);
        return _navigationService.GoToDayTasksAsync(day.Date);
    }

    /// <summary>
    /// Принудительно перезагружает календарь (полезно при возврате с других экранов).
    /// </summary>
    public void Reload() => GenerateCalendar();

    /// <summary>
    /// Заполняет коллекцию <see cref="Days"/> днями для отображения в сетке.
    /// Сетка начинается с понедельника и включает дни предыдущего/следующего месяца для заполнения недель.
    /// </summary>
    private void GenerateCalendar()
    {
        DateTime firstDayOfMonth = new(CurrentMonth.Year, CurrentMonth.Month, 1);
        int offset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;
        DateTime current = firstDayOfMonth.AddDays(-offset);
        DateTime end = current.AddDays(41);
        DateTime today = DateTime.Today;

        Dictionary<DateTime, Day> daysLookup = _dayService.GetDaysForRange(current, end);
        List<CalendarDay> newDays = [];

        while (newDays.Count < 42)
        {
            daysLookup.TryGetValue(current, out Day? day);
            newDays.Add(new CalendarDay
            {
                Date = current,
                IsCurrentMonth = current.Month == CurrentMonth.Month,
                IsToday = current == today,
                DayTypeName = day?.Type?.Name
            });
            current = current.AddDays(1);
        }

        Days = new ObservableCollection<CalendarDay>(newDays);
    }
}
