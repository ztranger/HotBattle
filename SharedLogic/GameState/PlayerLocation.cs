using System.Collections.Generic;
using SharedLogic.GameState.Buildings;
using Tanat.SharedLogic.GameState;
using Tanat.SharedLogic.GameState.Buildings;

namespace SharedLogic.GameState
{
    public class PlayerLocation
    {
        public Dictionary<string, SignalProp<MapItem>> Buildings { get; internal set; }
    }
}
