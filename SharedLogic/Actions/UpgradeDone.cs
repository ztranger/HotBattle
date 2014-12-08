using System;
using SharedLogic;
using SharedLogic.Actions;
using Tanat.SharedLogic.Defs;
using Tanat.SharedLogic.Defs.Buildings;
using Tanat.SharedLogic.GameState.Buildings;

namespace Tanat.SharedLogic.Actions
{
    public class UpgradeDone : GameAction
    {
        private readonly string _buildingId;

        public UpgradeDone(string buildingId)
        {
            _buildingId = buildingId;
        }

        internal override void Execute(Context context)
        {
            MapItem toUpgrade = context.State.Location.Buildings[_buildingId].Value;
            MapItemDef curDef = context.Defs.Buildings[toUpgrade.DefId];
            ObjectState gradeState = new ObjectState();
            gradeState.CurrentState = curDef.States[Const.UpgradeStateId].NextState;
            gradeState.StateStartTime = Time;
            toUpgrade.DefId.Value = curDef.UpgradeDefId;
            toUpgrade.State.Value = gradeState;
            context.State.Player.Expirience.Value += curDef.UpgradeExp;
        }
    }
}

