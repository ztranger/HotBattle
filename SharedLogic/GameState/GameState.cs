using System;
using Tanat.SharedLogic.GameState;

namespace SharedLogic.GameState
{
    public class GameState
    {
        public Action<GameState> Inited;
        public Player Player { get; internal set; }
        public PlayerLocation Location { get; internal set; }

        public int NextCommandId { get; internal set; }
        public long LastCommandTime { get; internal set; }

        //[JsonIgnore]
        internal LogicRandom Random { get; set; }

        //[JsonIgnore]
        //internal TimeoutQueue TimeoutQueue { get; private set; }

        public GameState()
        {
            NextCommandId = 0;
            //    TimeoutQueue = new TimeoutQueue();
        }

        internal void SendInited()
        {
            if (Inited != null)
                Inited(this);
        }

        internal void Update(long time)
        {
            if (time < LastCommandTime)
                throw new ApplicationException("invalid action time (less then last time: " + time + " < " + LastCommandTime + ")");

        /*while (TimeoutQueue.Count > 0)
        {
            BaseTimeout updatable = TimeoutQueue.Peek();
            if (updatable.ElapseAt > time)
                break;
            TimeoutQueue.Dequeue();
            updatable.Elapse();
        }*/

            NextCommandId++;
            LastCommandTime = time;
        }

    }
}
