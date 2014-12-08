using Tanat.SharedLogic.GameState;
using Tanat.SharedLogic.GameState.Buildings;

namespace SharedLogic.GameState.Buildings
{
    public class Farm : MapItem
    {
        public SignalProp<int> CurrentMoney { get; internal set; }
    }
}
