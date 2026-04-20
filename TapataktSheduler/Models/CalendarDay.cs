using Microsoft.Maui.Graphics;

namespace TapataktSheduler.Models;

/// <summary>
/// Элемент сетки календаря, представляющий один день для отображения.
/// </summary>
public sealed class CalendarDay
{
    /// <summary>
    /// Дата, которую представляет ячейка.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Флаг: день принадлежит текущему отображаемому месяцу (а не предыдущему/следующему).
    /// </summary>
    public bool IsCurrentMonth { get; set; }

    /// <summary>
    /// Флаг: день является сегодняшним.
    /// </summary>
    public bool IsToday { get; set; }

    /// <summary>
    /// Название типа дня, если он назначен (null, если не назначен).
    /// </summary>
    public string? DayTypeName { get; set; }

    /// <summary>
    /// Цвет фона ячейки: голубой для сегодня, иначе прозрачный.
    /// </summary>
    public Color BackgroundColor => IsToday ? Colors.LightBlue : Colors.Transparent;

    /// <summary>
    /// Цвет текста номера дня: чёрный для текущего месяца, серый для соседних.
    /// </summary>
    public Color TextColor => IsCurrentMonth ? Colors.Black : Colors.Gray;

    /// <summary>
    /// Начертание шрифта номера дня: жирный для сегодня, обычный для остальных.
    /// </summary>
    public FontAttributes DayFontAttributes => IsToday ? FontAttributes.Bold : FontAttributes.None;

    /// <summary>
    /// Размер шрифта номера дня: увеличенный для сегодня.
    /// </summary>
    public double DayFontSize => IsToday ? 22 : 18;
}
