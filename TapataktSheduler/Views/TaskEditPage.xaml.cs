using TapataktSheduler.Models;
using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница редактирования существующего дела.
/// </summary>
[QueryProperty(nameof(TaskId), "taskId")]
public partial class TaskEditPage : ContentPage
{
    private readonly TaskEditViewModel _viewModel;

    /// <summary>
    /// Идентификатор дела из параметров навигации.
    /// </summary>
    public string TaskId
    {
        set
        {
            if (Guid.TryParse(value, out Guid parsed))
                _viewModel.Initialize(parsed);
        }
    }

    /// <summary>
    /// Создаёт новый экземпляр страницы редактирования дела.
    /// </summary>
    /// <param name="viewModel">ViewModel редактирования дела.</param>
    public TaskEditPage(TaskEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        NameEntry.Unfocused += OnNameEntryUnfocused;
        FrequencyEntry.Unfocused += OnFrequencyEntryUnfocused;
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.AutoSave();
    }

    /// <summary>
    /// Сохраняет дело при потере фокуса полем названия.
    /// </summary>
    private void OnNameEntryUnfocused(object? sender, FocusEventArgs e)
        => _viewModel.AutoSave();

    /// <summary>
    /// Сохраняет дело при потере фокуса полем частоты повторов.
    /// </summary>
    private void OnFrequencyEntryUnfocused(object? sender, FocusEventArgs e)
        => _viewModel.AutoSave();

}
