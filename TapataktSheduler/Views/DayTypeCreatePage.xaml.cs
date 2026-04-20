using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница создания нового типа дня.
/// </summary>
public partial class DayTypeCreatePage : ContentPage
{
    /// <summary>
    /// Создаёт новый экземпляр страницы создания типа дня.
    /// </summary>
    /// <param name="viewModel">ViewModel создания типа дня.</param>
    public DayTypeCreatePage(DayTypeCreateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
