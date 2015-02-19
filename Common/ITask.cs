using System;

namespace HPG.Common
{
    public interface ITask<T> where T : ITask<T>
    {
        event Action<T> Complete;
        event Action<T> Error;
    
        int Priority { get; }

        void Start();
        void Stop();
    }
}
