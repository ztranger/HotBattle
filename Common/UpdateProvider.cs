using System;
using System.Linq;
using UnityEngine;

namespace HPG.Common
{
    public class UpdateProvider : MonoBehaviour
    {
        private event Action OnUpdate;

        public void SubscribeUpdate(Action update)
        {
            if (OnUpdate != null)
            {
                Delegate[] invocationList = OnUpdate.GetInvocationList();
                if (invocationList.Contains(update))
                    return;
            }
            OnUpdate += update;
        }

        public void UnSubscribeUpdate(Action update)
        {
            OnUpdate -= update;
        }

        #region singleton
        private static volatile UpdateProvider _curMgr;
        private UpdateProvider()
        {
        }

        public static UpdateProvider Instance
        {
            get
            {
                if (_curMgr == null)
                {
                    if(FindObjectOfType<UpdateProvider>() != null)
                        throw new Exception("Dont create UpdateProvider manualy");
                    GameObject go = new GameObject(typeof(UpdateProvider).Name);
                    _curMgr = go.AddComponent<UpdateProvider>();
                    DontDestroyOnLoad(go);
                }
                return _curMgr;
            }
        }

        #endregion
        private void Update()
        {
            if (OnUpdate != null)
                OnUpdate();
        }
    }
}
