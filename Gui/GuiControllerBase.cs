using System;
using UnityEngine;

namespace HPG.Gui
{
    /// <summary>
    /// Контроллер префаба ГУИ
    /// </summary>
    public abstract class GuiControllerBase : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            _disposed = true;
        }
    }
}

