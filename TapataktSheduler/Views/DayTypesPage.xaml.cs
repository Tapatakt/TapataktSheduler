using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница списка типов дней.
/// </summary>
public partial class DayTypesPage : ContentPage
{
    private readonly DayTypesViewModel _viewModel;

    /// <summary>
    /// Создаёт новый экземпляр страницы типов дней с внедрением ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel списка типов дней.</param>
    public DayTypesPage(DayTypesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadDayTypes();
    }
}
