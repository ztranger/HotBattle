using System;
using System.Collections.Generic;
using SharedLogic;

namespace Tanat.SharedLogic.GameState
{
    public class DictionarySignalProp<TKey, TValue> : Dictionary<TKey, SignalProp<TValue>>
    {
        public event Action<TKey, TValue> Changed;
        public event Action<TKey, TValue> Added;
        public event Action<TKey, TValue> Removed;

        public void Add(TKey key, TValue value)
        {
            base.Add(key, new SignalProp<TValue>(value));
            if (Added != null)
                Context.Instance.Emit(() => Added(key, value));
        }

        public new bool Remove(TKey key)
        {
            TValue value = base[key];
            if (base.Remove(key))
            {
                if(Removed != null)
                    Context.Instance.Emit(() => Removed(key, value));
                return true;
            }
            return false;
        }

        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
                if (Changed != null)
                    Context.Instance.Emit(() => Changed(key, value));
            }
        } 
    }
}
