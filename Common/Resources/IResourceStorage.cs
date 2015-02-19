using System;

namespace HPG.Common.Resources
{
    public interface IResourceStorage
    {
        IResource<T> Get<T>(string assetName) where T : UnityEngine.Object;
        void LoadBundle(string bundleName, Action<string> bundleLoaded);
        void UnLoadBundle(string bundleName);
    }
}
