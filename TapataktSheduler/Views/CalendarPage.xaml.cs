using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница календаря для выбора дня.
/// </summary>
public partial class CalendarPage : ContentPage
{
    private readonly CalendarViewModel _viewModel;

    /// <summary>
    /// Создаёт новый экземпляр страницы календаря с внедрением ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel календаря.</param>
    public CalendarPage(CalendarViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Reload();
    }

    /// <summary>
    /// Обрабатывает выбор дня в CollectionView и переходит к экрану дела.
    /// </summary>
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Models.CalendarDay day)
        {
            _viewModel.SelectDayCommand.Execute(day);
            if (sender is CollectionView collectionView)
                collectionView.SelectedItem = null;
        }
    }
}
