using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TapataktSheduler.Models;
using TapataktSheduler.Services;

namespace TapataktSheduler.ViewModels;

/// <summary>
/// ViewModel экрана создания нового типа дня.
/// </summary>
public sealed partial class DayTypeCreateViewModel : BaseViewModel
{
    private readonly IDayTypeService _dayTypeService;
    private readonly INavigationService _navigationService;

    /// <summary>
    /// Название нового типа дня.
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// Создаёт новый экземпляр ViewModel.
    /// </summary>
    /// <param name="dayTypeService">Сервис типов дней.</param>
    /// <param name="navigationService">Сервис навигации.</param>
    public DayTypeCreateViewModel(IDayTypeService dayTypeService, INavigationService navigationService)
    {
        _dayTypeService = dayTypeService;
        _navigationService = navigationService;
    }

    /// <summary>
    /// Создаёт тип дня и возвращается на предыдущий экран.
    /// </summary>
    [RelayCommand]
    private Task CreateAsync()
    {
        ArgumentException.ThrowIfNullOrEmpty(Name);

        DayType dayType = new() { Name = Name };
        _dayTypeService.SaveDayType(dayType);
        return _navigationService.GoBackAsync();
    }

    /// <summary>
    /// Создаёт тип дня и подменяет текущий экран экраном редактирования.
    /// </summary>
    [RelayCommand]
    private Task CreateAndEditAsync()
    {
        ArgumentException.ThrowIfNullOrEmpty(Name);

        DayType dayType = new() { Name = Name };
        _dayTypeService.SaveDayType(dayType);
        return _navigationService.ReplaceToDayTypeEditAsync(dayType.Id);
    }
}
