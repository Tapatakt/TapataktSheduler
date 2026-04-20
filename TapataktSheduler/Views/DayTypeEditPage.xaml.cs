using TapataktSheduler.Models;
using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница редактирования существующего типа дня.
/// </summary>
[QueryProperty(nameof(DayTypeId), "dayTypeId")]
public partial class DayTypeEditPage : ContentPage
{
    private readonly DayTypeEditViewModel _viewModel;

    /// <summary>
    /// Идентификатор редактируемого типа дня, получаемый из параметров навигации.
    /// </summary>
    public string DayTypeId
    {
        set
        {
            if (Guid.TryParse(value, out Guid parsed))
                _viewModel.Initialize(parsed);
        }
    }

    /// <summary>
    /// Создаёт новый экземпляр страницы редактирования типа дня.
    /// </summary>
    /// <param name="viewModel">ViewModel редактирования типа дня.</param>
    public DayTypeEditPage(DayTypeEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        NameEntry.Unfocused += OnNameEntryUnfocused;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Reload();
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.AutoSave();
    }

    /// <summary>
    /// Сохраняет название при потере фокуса.
    /// </summary>
    private void OnNameEntryUnfocused(object? sender, FocusEventArgs e)
        => _viewModel.AutoSave();

}
