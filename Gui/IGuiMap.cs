namespace HPG.Gui
{
    /// <summary>
    /// Список соответствия класса контроллера к префабу ГУИ
    /// </summary>
    public interface IGuiMap
    {
        /// <summary>
        /// Предоставляет название префаба ГУИ по заданному классу контроллера
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string GetPrefabName<T>() where T : GuiControllerBase;

        /// <summary>
        /// Название префаба модальной блочащей подложки
        /// </summary>
        /// <returns></returns>
        string GetModalPrefabName();
    }
}
