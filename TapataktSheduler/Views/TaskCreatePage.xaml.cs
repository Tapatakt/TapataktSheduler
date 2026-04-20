using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница создания нового дела.
/// </summary>
[QueryProperty(nameof(DateParameter), "date")]
[QueryProperty(nameof(DayTypeId), "dayTypeId")]
public partial class TaskCreatePage : ContentPage
{
    private readonly TaskCreateViewModel _viewModel;
    private string _date = string.Empty;
    private string _dayTypeId = string.Empty;

    /// <summary>
    /// Дата для автопривязки из параметров навигации.
    /// </summary>
    public string DateParameter
    {
        set
        {
            _date = value;
            TryInitialize();
        }
    }

    /// <summary>
    /// Идентификатор типа дня для автопривязки из параметров навигации.
    /// </summary>
    public string DayTypeId
    {
        set
        {
            _dayTypeId = value;
            TryInitialize();
        }
    }

    /// <summary>
    /// Создаёт новый экземпляр страницы создания дела.
    /// </summary>
    /// <param name="viewModel">ViewModel создания дела.</param>
    public TaskCreatePage(TaskCreateViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _viewModel.Initialize(null, null);
    }

    private void TryInitialize()
    {
        DateTime? date = DateTime.TryParse(_date, out DateTime d) ? d : null;
        Guid? dayTypeId = Guid.TryParse(_dayTypeId, out Guid dt) ? dt : null;
        _viewModel.Initialize(date, dayTypeId);
    }
}
