using System;
using HPG.Common.Resources;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HPG.Gui
{
    /// <summary>
    /// Фабрика ГУИ объектов
    /// </summary>
    public class GuiFactory
    {
        protected readonly IResourceStorage _resources;
        protected readonly IGuiMap _guiMap;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resources">Ресурсная система</param>
        /// <param name="guiMap">Список ГУИ элементов</param>
        public GuiFactory(IResourceStorage resources, IGuiMap guiMap)
        {
            _resources = resources;
            _guiMap = guiMap;
        }

        /// <summary>
        /// Создает геймобъект ГУИ по классу контроллера
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual GameObject CreateGuiObject<T>() where T : GuiControllerBase
        {
            if (_resources == null)
                throw new Exception("Resource system is not initialized");
            string prefabName = _guiMap.GetPrefabName<T>();
            if (string.IsNullOrEmpty(prefabName))
                throw new Exception(string.Format("No prefab for {0} specified in GuiMap", typeof(T)));
            IResource<GameObject> resource = _resources.Get<GameObject>(prefabName);
            if (resource == null)
                throw new Exception("Couldn't get resource");
            Object prefab = resource.Asset;
            if (prefab == null)
                throw new Exception(string.Format("Prefab for UI element {0} is null", prefabName));
            return Object.Instantiate(prefab) as GameObject;
        }

        /// <summary>
        /// Создает геймобъект модальной подложки
        /// </summary>
        /// <returns></returns>
        public virtual GameObject CreateModalObject()
        {
            return Object.Instantiate(_resources.Get<GameObject>(_guiMap.GetModalPrefabName()).Asset) as GameObject;
        }
    }
}
