using CommunityToolkit.Mvvm.ComponentModel;

namespace TapataktSheduler.Models;

/// <summary>
/// Тип дня (например, "Рабочий", "Выходной", "Спорт").
/// Содержит идентификатор и название; связанные дни и дела запрашиваются через сервисы.
/// </summary>
public sealed partial class DayType : ObservableObject
{
    /// <summary>
    /// Уникальный идентификатор типа дня.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название типа дня.
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;
}
