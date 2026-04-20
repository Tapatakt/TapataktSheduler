namespace TapataktSheduler.Services;

/// <summary>
/// Сервис управления типами дней.
/// </summary>
public interface IDayTypeService
{
    /// <summary>
    /// Возвращает все типы дней.
    /// </summary>
    List<Models.DayType> GetDayTypes();

    /// <summary>
    /// Возвращает тип дня по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор типа дня.</param>
    /// <returns>Найденный тип дня или null.</returns>
    Models.DayType? GetDayType(Guid id);

    /// <summary>
    /// Сохраняет тип дня (добавление или обновление).
    /// </summary>
    /// <param name="dayType">Тип дня для сохранения.</param>
    void SaveDayType(Models.DayType dayType);

    /// <summary>
    /// Удаляет тип дня.
    /// </summary>
    /// <param name="id">Идентификатор удаляемого типа.</param>
    void DeleteDayType(Guid id);

    /// <summary>
    /// Проверяет, можно ли удалить тип дня (нет будущих дней этого типа).
    /// </summary>
    /// <param name="id">Идентификатор типа дня.</param>
    /// <returns>True, если удаление возможно; иначе False.</returns>
    bool CanDeleteDayType(Guid id);
}
