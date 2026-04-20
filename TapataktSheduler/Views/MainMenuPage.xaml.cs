using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Главная страница приложения с меню навигации и списком дел на сегодня.
/// </summary>
public partial class MainMenuPage : ContentPage
{
    private readonly MainMenuViewModel _viewModel;

    /// <summary>
    /// Создаёт новый экземпляр главной страницы с внедрением ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel главного меню.</param>
    public MainMenuPage(MainMenuViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadTodayTasks();
    }
}
