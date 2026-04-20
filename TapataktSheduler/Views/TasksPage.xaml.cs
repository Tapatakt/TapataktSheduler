using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница списка всех дел.
/// </summary>
public partial class TasksPage : ContentPage
{
    private readonly TasksViewModel _viewModel;

    /// <summary>
    /// Создаёт новый экземпляр страницы списка дел с внедрением ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel списка дел.</param>
    public TasksPage(TasksViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadTasks();
    }
}
