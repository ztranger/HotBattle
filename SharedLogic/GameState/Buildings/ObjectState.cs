namespace Tanat.SharedLogic.GameState.Buildings
{
    public class ObjectState
    {
        public SignalProp<string> CurrentState { get; internal set; }
        public SignalProp<long> StateStartTime { get; internal set; }
    }
}
