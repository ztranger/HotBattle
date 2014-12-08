namespace Tanat.SharedLogic.Defs
{
    public class StateDef
    {
        public long Duration { get; internal set; }
        public string NextState { get; internal set; }
        public string PrefabName { get; internal set; }
    }
}
