using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана списка типов дней.
/// Управляет загрузкой, добавлением и переходом к редактированию типов.
/// </summary>
public sealed partial class DayTypesViewModel : BaseViewModel
{
    private readonly IDayTypeService _dayTypeService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Коллекция типов дней для отображения в списке.
    /// </summary>
    public ObservableCollection<DayType> DayTypes { get; } = new();

    /// <summary>
    /// Создаёт новый экземпляр ViewModel списка типов дней.
    /// </summary>
    /// <param name="dayTypeService">Сервис типов дней.</param>
    /// <param name="navigationService">Сервис навигации.</param>
    public DayTypesViewModel(IDayTypeService dayTypeService, INavigationService navigationService)
    {
        _dayTypeService = dayTypeService;
        _navigationService = navigationService;
        LoadDayTypes();
    }

    /// <summary>
    /// Перезагружает список типов дней из хранилища.
    /// </summary>
    [RelayCommand]
    public void LoadDayTypes()
    {
        DayTypes.Clear();
        foreach (DayType dayType in _dayTypeService.GetDayTypes().OrderBy(dt => dt.Name))
            DayTypes.Add(dayType);
    }

    /// <summary>
    /// Переходит на экран создания нового типа дня.
    /// </summary>
    [RelayCommand]
    private Task AddDayTypeAsync() => _navigationService.GoToDayTypeCreateAsync();

    /// <summary>
    /// Переходит на экран редактирования выбранного типа дня.
    /// </summary>
    /// <param name="dayType">Тип дня для редактирования.</param>
    [RelayCommand]
    private Task EditDayTypeAsync(DayType dayType)
    {
        ArgumentNullException.ThrowIfNull(dayType);
        return _navigationService.GoToDayTypeEditAsync(dayType.Id);
    }
}
