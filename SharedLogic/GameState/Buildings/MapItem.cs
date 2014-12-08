using JsonFx.Json;
using SharedLogic.GameState;
using SharedLogic.GameState.Buildings;

namespace Tanat.SharedLogic.GameState.Buildings
{
    [JsonClass("MapItemType")]
    public class MapItem
    {
        public string MapItemType
        {
            get { return GetType().FullName; }
            set { }
        }

        public SignalProp<string> DefId { get; internal set; }
        
        public SignalProp<ObjectState> State { get; internal set; }
    }
}
