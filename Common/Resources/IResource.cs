using System;

namespace HPG.Common.Resources
{
    public interface IResource<T> : IDisposable where T : UnityEngine.Object
    {
        void GetAsset(Action<T> onLoaded);
        T Asset { get; }
    }

}
