using System.Collections.Generic;

namespace Tanat.SharedLogic.GameState
{
    public class Player
    {
        public SignalProp<long> Expirience { get; internal set; }
        public DictionarySignalProp<string, int> GameBalance { get; internal set; }
        public SignalProp<int> RealBalance { get; internal set; }
        public DictionarySignalProp<string, string> AvailableHeroes { get; internal set; }
    }
}
