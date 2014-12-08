using System.Collections.Generic;
using SharedLogic.GameState;
using Tanat.SharedLogic.Defs.Buildings;

namespace SharedLogic.Defs.Buildings
{
    public class FarmDef : MapItemDef
    {
        public int Capacity { get; protected set; }
        public int ResPerSecond { get; protected set; }
        public string ResourceId { get; protected set; }
        public CurrencyType CurrencyType { get; private set; }
    }
}
