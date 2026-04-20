using TapataktSheduler.ViewModels;

namespace TapataktSheduler.Views;

/// <summary>
/// Страница отображения и управления делами на конкретный день.
/// </summary>
[QueryProperty(nameof(DateParameter), "date")]
public partial class DayTasksPage : ContentPage
{
    private readonly DayTasksViewModel _viewModel;

    /// <summary>
    /// Параметр даты из навигации. Устанавливается автоматически при переходе на страницу.
    /// </summary>
    public string DateParameter
    {
        set
        {
            if (DateTime.TryParse(value, out DateTime date))
            {
                DayTypePicker.SelectedIndexChanged -= OnDayTypePickerSelectedIndexChanged;
                _viewModel.Initialize(date);
                DayTypePicker.SelectedIndexChanged += OnDayTypePickerSelectedIndexChanged;
            }
        }
    }

    /// <summary>
    /// Создаёт новый экземпляр страницы дня с внедрением ViewModel.
    /// </summary>
    /// <param name="viewModel">ViewModel дня.</param>
    public DayTasksPage(DayTasksViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _viewModel.TasksReloaded += OnTasksReloaded;
        DayTypePicker.SelectedIndexChanged += OnDayTypePickerSelectedIndexChanged;
        CommentEditor.Unfocused += OnCommentEditorUnfocused;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        DayTypePicker.SelectedIndexChanged -= OnDayTypePickerSelectedIndexChanged;
        _viewModel.Reload();
        DayTypePicker.SelectedIndexChanged += OnDayTypePickerSelectedIndexChanged;
    }

    /// <inheritdoc />
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.AutoSave();
    }

    /// <summary>
    /// Сохраняет день при смене типа через нативный Picker.
    /// </summary>
    private void OnDayTypePickerSelectedIndexChanged(object? sender, EventArgs e)
        => _viewModel.AutoSave();

    /// <summary>
    /// Сохраняет день при потере фокуса комментарием.
    /// </summary>
    private void OnCommentEditorUnfocused(object? sender, FocusEventArgs e)
        => _viewModel.AutoSave();

    /// <summary>
    /// Вызывает пересчёт размеров ScrollView после обновления списков BindableLayout.
    /// </summary>
    private void OnTasksReloaded()
    {
        Dispatcher.Dispatch(() =>
        {
            if (Content is ScrollView scrollView)
                scrollView.Content?.InvalidateMeasure();
        });
    }
}
