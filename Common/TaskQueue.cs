using System;
using System.Collections.Generic;

namespace HPG.Common
{
    /// <summary>
    /// очередь выполнения тасков. является локальной очередью(не используется как единая очередь дя всех тасков системы)
    /// </summary>
    public class TaskQueue<TTask> where TTask : ITask<TTask>
    {
        public event Action Complete;
        public event Action<TTask> Error;

        protected readonly List<TTask> _queuedTasks = new List<TTask>();
        protected readonly List<TTask> _runningTasks = new List<TTask>();

        protected bool _running;

        protected int _maxThreads;
        protected readonly bool _usePriority;
        protected readonly bool _cancelOnError;

        public ICollection<TTask> GetRunningTasks()
        {
            return _runningTasks.ToArray();
        }

        public int RunningTasks
        {
            get { return _runningTasks.Count; }
        }

        public int QueuedTasks
        {
            get { return _queuedTasks.Count; }
        }

        public bool IsRunning
        {
            get { return _running; }
        }

        public TaskQueue()
            : this(1, false, false)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxThreads">Максимальное число одновременно выполняющихся асинхронных заданий</param>
        /// <param name="usePriority">Использовать ли приоритеты</param>
        /// <param name="cancelOnError">Прерывать ли выполнение очереди, если в одной из тасок произошла ошибка</param>
        public TaskQueue(int maxThreads, bool usePriority, bool cancelOnError)
        {
            _maxThreads = (maxThreads > 0) ? maxThreads : 1;
            _usePriority = usePriority;
            _cancelOnError = cancelOnError;

            _running = false;
        }

        /// <summary>
        /// Запустить выполнение очереди заданий
        /// </summary>
        public void Start()
        {
            if (_running)
                return;
            _running = true;
            CheckQueue();
        }

        /// <summary>
        /// Остановить выполнение очереди заданий
        /// </summary>
        public void Stop()
        {
            if (!_running)
                return;
            _running = false;
        }

        /// <summary>
        /// Немедленно остановить очередь и все выполняемые задания
        /// </summary>
        public void StopImmediately()
        {
            foreach (TTask runningTask in _runningTasks)
            {
                Unsubscribe(runningTask);
                runningTask.Stop();
            }
            _runningTasks.Clear();
            Stop();
        }

        /// <summary>
        /// Удалить все отложенные задания
        /// </summary>
        public void Clear()
        {
            _queuedTasks.Clear();
        }

        /// <summary>
        /// Добавить задание в очередь
        /// </summary>
        /// <param name="task"></param>
        public virtual TTask Add(TTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            if (!_usePriority || _queuedTasks.Count == 0 || _queuedTasks[_queuedTasks.Count - 1].Priority > task.Priority)
            {
                _queuedTasks.Add(task);
            }
            else
            {
                for (int i = _queuedTasks.Count - 1; i < 0; i--)
                {
                    if (_queuedTasks[i].Priority < task.Priority)
                        continue;
                    _queuedTasks.Insert(i, task);
                    return task;
                }
                _queuedTasks.Insert(0, task);
            }

            // Запускаем задачу, если возможно
            CheckQueue();
            return task;
        }

        /// <summary>
        /// Убрать задание из очереди (если задание выполняется, оно будет остановлено принудительно)
        /// </summary>
        /// <param name="task"></param>
        public void Remove(TTask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            // Ищем задание в списке ожидающих
            if (_queuedTasks.Remove(task))
                return;

            // Ищем в выполняемых
            int index = _runningTasks.IndexOf(task);
            if (index == -1)
                return;

            // Останавливаем задачу
            Unsubscribe(task);
            _runningTasks.Remove(task);
            task.Stop();

            // Запускаем задачу, если возможно
            CheckQueue();
        }

        private void CheckQueue()
        {
            // пока есть ожидающие задания и выполняемых заданий меньше
            // чем число потоков - набиваем буфер и стартуем задания
            while (_running && (_queuedTasks.Count > 0) && (_runningTasks.Count < _maxThreads))
            {
                StartNextTask();
            }

            // Если нет выполняемых заданий, значит очередь завершена
            if (_running && _runningTasks.Count == 0)
            {
                _running = false;
                if (Complete != null)
                    Complete();
            }
        }

        private void StartNextTask()
        {
            if (!_running || _queuedTasks.Count == 0)
                return;

            TTask task = _queuedTasks[0];
            _queuedTasks.RemoveAt(0);

            task.Complete += OnTaskComplete;
            task.Error += OnTaskError;

            _runningTasks.Add(task);
            task.Start();
        }

        private void Unsubscribe(TTask task)
        {
            task.Complete -= OnTaskComplete;
            task.Error -= OnTaskError;
        }

        private void OnTaskComplete(TTask task)
        {
            if (task == null)
                return;

            Unsubscribe(task);
            _runningTasks.Remove(task);

            CheckQueue();
        }

        private void OnTaskError(TTask task)
        {
            Unsubscribe(task);
            _runningTasks.Remove(task);

            if (_cancelOnError)
            {
                StopImmediately();
                if (Error != null)
                    Error(task);
                return;
            }

            CheckQueue();
        }
    }
}
