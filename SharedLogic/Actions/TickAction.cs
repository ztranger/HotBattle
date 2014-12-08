using SharedLogic;
using SharedLogic.Actions;

namespace Tanat.SharedLogic.Actions
{
    public class TickAction : GameAction
    {
        protected int _objectId;

        public TickAction(int objectId)
        {
            _objectId = objectId;
        }

        internal override void Execute(Context context)
        {
            //context.StateDef.
            //CurrentState = Context.Instance.Defs.GetBuildingDescription<MapItemDef>(TypeId).States[CurrentState].NextState;
        }
    }
}
