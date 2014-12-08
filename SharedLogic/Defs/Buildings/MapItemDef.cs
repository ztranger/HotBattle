using System.Collections.Generic;
using JsonFx.Json;
using SharedLogic.Defs;

namespace Tanat.SharedLogic.Defs.Buildings
{
    [JsonClass("DefType")]
    public class MapItemDef 
    {
        public string Name { get; protected set; }
        public string Desc { get; protected set; }
        public string UpgradeDefId { get; protected set; }
        public int UpgradeExp { get; protected set; }
        public Dictionary<string, int> UpgradeCost { get; protected set; }
        public VectorDef Position { get; protected set; }
        //public string DefaultState { get; protected set; }
        public Dictionary<string, StateDef> States { get; protected set; }
    }
}
