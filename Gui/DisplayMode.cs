namespace HPG.Gui
{
    /// <summary>
    /// Режим отображения ГУИ
    /// </summary>
    public enum DisplayMode
    {
        /// <summary>
        /// Постоянный, закрывается только при соответсвующем вызове Close
        /// </summary>
        Persistent,

        /// <summary>
        /// Режим скрина, закрывается автоматом при показе другого скрина через Show
        /// </summary>
        Screen,

        /// <summary>
        /// Модальный диалог, добавляется поверх открытых скринов/диалогов
        /// </summary>
        DialogAddFront,

        /// <summary>
        /// Модальный диалог, закрывает текущие отображаемые диалоги
        /// </summary>
        DialogCloseOthers,

        /// <summary>
        /// Модальный диалог, добавляется в очередь показа окон, единовременно отображается только одно окно данного режима
        /// </summary>
        DialogQueued
    }
}
