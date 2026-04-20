namespace TapataktSheduler.Services;

/// <summary>
/// Реализация сервиса навигации на базе Shell с передачей параметров через query string.
/// </summary>
public sealed class NavigationService : INavigationService
{
    /// <inheritdoc />
    public Task GoToMainMenuAsync() => NavigateAsync("//MainMenu");

    /// <inheritdoc />
    public Task GoToCalendarAsync() => NavigateAsync("Calendar");

    /// <inheritdoc />
    public Task GoToDayTasksAsync(DateTime date) =>
        NavigateAsync($"DayTasks?date={date:yyyy-MM-dd}");

    /// <inheritdoc />
    public Task GoToTaskCreateAsync(DateTime? date = null, Guid? dayTypeId = null)
    {
        List<string> parameters = [];
        if (date.HasValue)
            parameters.Add($"date={date.Value:yyyy-MM-dd}");
        if (dayTypeId.HasValue)
            parameters.Add($"dayTypeId={dayTypeId.Value}");

        string query = parameters.Count > 0 ? "?" + string.Join("&", parameters) : string.Empty;
        return NavigateAsync($"TaskCreate{query}");
    }

    /// <inheritdoc />
    public Task GoToTaskEditAsync(Guid taskId) =>
        NavigateAsync($"TaskEdit?taskId={taskId}");

    /// <inheritdoc />
    public async Task ReplaceToTaskEditAsync(Guid taskId)
    {
        IReadOnlyList<Page> stack = Shell.Current.Navigation.NavigationStack;
        Page? current = stack.Count > 0 ? stack[^1] : null;

        await NavigateAsync($"TaskEdit?taskId={taskId}");

        if (current != null && Shell.Current.Navigation.NavigationStack.Contains(current))
        {
            Shell.Current.Navigation.RemovePage(current);
        }
    }

    /// <inheritdoc />
    public Task GoToTasksAsync() => NavigateAsync("Tasks");

    /// <inheritdoc />
    public Task GoToDayTypesAsync() => NavigateAsync("DayTypes");

    /// <inheritdoc />
    public Task GoToDayTypeCreateAsync() => NavigateAsync("DayTypeCreate");

    /// <inheritdoc />
    public Task GoToDayTypeEditAsync(Guid dayTypeId) =>
        NavigateAsync($"DayTypeEdit?dayTypeId={dayTypeId}");

    /// <inheritdoc />
    public async Task ReplaceToDayTypeEditAsync(Guid dayTypeId)
    {
        IReadOnlyList<Page> stack = Shell.Current.Navigation.NavigationStack;
        Page? current = stack.Count > 0 ? stack[^1] : null;

        await NavigateAsync($"DayTypeEdit?dayTypeId={dayTypeId}");

        if (current != null && Shell.Current.Navigation.NavigationStack.Contains(current))
        {
            Shell.Current.Navigation.RemovePage(current);
        }
    }

    /// <inheritdoc />
    public Task GoToSettingsAsync() => NavigateAsync("Settings");

    /// <inheritdoc />
    public Task GoBackAsync()
    {
        if (Shell.Current == null)
            return Task.CompletedTask;

        return Shell.Current.Navigation.PopAsync();
    }

    /// <summary>
    /// Выполняет навигацию по указанному маршруту.
    /// </summary>
    /// <param name="route">Целевой маршрут.</param>
    private static async Task NavigateAsync(string route)
    {
        if (Shell.Current == null)
            return;

        await Shell.Current.GoToAsync(route);
    }
}
