using TapataktSheduler.Models;

namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса диалогов через встроенный механизм .NET MAUI.
/// </summary>
public sealed class DialogService : IDialogService
{
    /// <inheritdoc />
    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        Page? page = Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[0].Page
            : null;

        if (page == null)
            return Task.CompletedTask;

        return page.DisplayAlertAsync(title, message, cancel);
    }

    /// <inheritdoc />
    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Да", string cancel = "Нет")
    {
        Page? page = Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[0].Page
            : null;

        if (page == null)
            return Task.FromResult(false);

        return page.DisplayAlertAsync(title, message, accept, cancel);
    }

    /// <inheritdoc />
    public Task<TimeSpan?> ShowTimePickerAsync(TimeSpan? initialTime)
    {
#if ANDROID
        return ShowAndroidTimePickerAsync(initialTime);
#else
        return Task.FromResult<TimeSpan?>(null);
#endif
    }

    /// <inheritdoc />
    public Task<DateTime?> ShowDatePickerAsync(DateTime? initialDate)
    {
#if ANDROID
        return ShowAndroidDatePickerAsync(initialDate);
#else
        return Task.FromResult<DateTime?>(null);
#endif
    }

    /// <inheritdoc />
    public Task<Guid?> ShowDayTypePickerAsync(List<DayType> dayTypes)
    {
#if ANDROID
        return ShowAndroidDayTypePickerAsync(dayTypes);
#else
        return Task.FromResult<Guid?>(null);
#endif
    }

    /// <inheritdoc />
    public Task<Guid?> ShowTaskPickerAsync(List<PlannedTask> tasks)
    {
#if ANDROID
        return ShowAndroidTaskPickerAsync(tasks);
#else
        return Task.FromResult<Guid?>(null);
#endif
    }

#if ANDROID
    /// <summary>
    /// Отображает нативный Android-диалог выбора времени.
    /// </summary>
    /// <param name="initialTime">Начальное время.</param>
    /// <returns>Выбранное время или null при отмене.</returns>
    private static Task<TimeSpan?> ShowAndroidTimePickerAsync(TimeSpan? initialTime)
    {
        TaskCompletionSource<TimeSpan?> tcs = new();

        Android.App.Activity? activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity == null)
        {
            tcs.TrySetResult(null);
            return tcs.Task;
        }

        TimeSpan time = initialTime ?? TimeSpan.Zero;
        Android.App.TimePickerDialog dialog = new(
            activity,
            (sender, e) => tcs.TrySetResult(new TimeSpan(e.HourOfDay, e.Minute, 0)),
            time.Hours,
            time.Minutes,
            true);

        dialog.SetButton(
            (int)Android.Content.DialogButtonType.Negative,
            activity.GetString(Android.Resource.String.Cancel),
            (sender, args) => tcs.TrySetResult(null));

        dialog.CancelEvent += (sender, e) => tcs.TrySetResult(null);
        dialog.Show();
        return tcs.Task;
    }

    /// <summary>
    /// Отображает нативный Android-диалог выбора даты.
    /// </summary>
    /// <param name="initialDate">Начальная дата.</param>
    /// <returns>Выбранная дата или null при отмене.</returns>
    private static Task<DateTime?> ShowAndroidDatePickerAsync(DateTime? initialDate)
    {
        TaskCompletionSource<DateTime?> tcs = new();

        Android.App.Activity? activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity == null)
        {
            tcs.TrySetResult(null);
            return tcs.Task;
        }

        DateTime date = initialDate ?? DateTime.Today;
        Android.App.DatePickerDialog dialog = new(
            activity,
            (sender, e) => tcs.TrySetResult(e.Date),
            date.Year,
            date.Month - 1,
            date.Day);

        dialog.SetButton(
            (int)Android.Content.DialogButtonType.Negative,
            activity.GetString(Android.Resource.String.Cancel),
            (sender, args) => tcs.TrySetResult(null));

        dialog.CancelEvent += (sender, e) => tcs.TrySetResult(null);
        dialog.Show();
        return tcs.Task;
    }

    /// <summary>
    /// Отображает нативный Android-диалог со списком типов дней.
    /// </summary>
    /// <param name="dayTypes">Список типов дней.</param>
    /// <returns>Идентификатор выбранного типа или null при отмене.</returns>
    private static Task<Guid?> ShowAndroidDayTypePickerAsync(List<DayType> dayTypes)
    {
        TaskCompletionSource<Guid?> tcs = new();

        Android.App.Activity? activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity == null)
        {
            tcs.TrySetResult(null);
            return tcs.Task;
        }

        string[] items = dayTypes.Select(dt => dt.Name).ToArray();
        Android.App.AlertDialog.Builder builder = new(activity);
        builder.SetTitle("Выберите тип дня");
        builder.SetItems(items, (sender, e) => tcs.TrySetResult(dayTypes[e.Which].Id));
        builder.SetNegativeButton(Android.Resource.String.Cancel, (sender, e) => tcs.TrySetResult(null));

        Android.App.AlertDialog? dialog = builder.Create();
        if (dialog != null)
        {
            dialog.CancelEvent += (sender, e) => tcs.TrySetResult(null);
            dialog.Show();
        }
        return tcs.Task;
    }

    /// <summary>
    /// Отображает нативный Android-диалог со списком дел.
    /// </summary>
    /// <param name="tasks">Список дел.</param>
    /// <returns>Идентификатор выбранного дела или null при отмене.</returns>
    private static Task<Guid?> ShowAndroidTaskPickerAsync(List<PlannedTask> tasks)
    {
        TaskCompletionSource<Guid?> tcs = new();

        Android.App.Activity? activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        if (activity == null)
        {
            tcs.TrySetResult(null);
            return tcs.Task;
        }

        string[] items = tasks.Select(t => t.Name).ToArray();
        Android.App.AlertDialog.Builder builder = new(activity);
        builder.SetTitle("Выберите дело");
        builder.SetItems(items, (sender, e) => tcs.TrySetResult(tasks[e.Which].Id));
        builder.SetNegativeButton(Android.Resource.String.Cancel, (sender, e) => tcs.TrySetResult(null));

        Android.App.AlertDialog? dialog = builder.Create();
        if (dialog != null)
        {
            dialog.CancelEvent += (sender, e) => tcs.TrySetResult(null);
            dialog.Show();
        }
        return tcs.Task;
    }
#endif
}
