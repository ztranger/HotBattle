using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedLogic;
using SharedLogic.Actions;
using Tanat.SharedLogic.Defs;
using Tanat.SharedLogic.Defs.Buildings;
using Tanat.SharedLogic.GameState;
using Tanat.SharedLogic.GameState.Buildings;
using UnityEngine;

namespace Tanat.SharedLogic.Actions
{
	class UpgradeBuilding : GameAction
    {
        private readonly string _buildingId;

        public UpgradeBuilding(string buildingId)
        {
            _buildingId = buildingId;
        }

        internal override void Execute(Context context)
        {
            MapItem toUpgrade = context.State.Location.Buildings[_buildingId].Value;
            if(toUpgrade.State.Value.CurrentState.Value != Const.IdleStateId)
                throw new Exception("can not upgrade building not in idle state");
            MapItemDef curDef = context.Defs.Buildings[toUpgrade.DefId];
            if(string.IsNullOrEmpty(curDef.UpgradeDefId))
                throw new Exception("can not upgrade last grade building");
            if(!curDef.States.ContainsKey(Const.UpgradeStateId))
                throw new Exception("can not upgrade building without upgrade def");
            foreach (KeyValuePair<string, int> resource in curDef.UpgradeCost)
            {
                if (!context.State.Player.GameBalance.ContainsKey(resource.Key) ||
                    context.State.Player.GameBalance[resource.Key] < resource.Value)
                {
                    throw new Exception("no resources " + resource.Key);
                }
            }
            foreach (KeyValuePair<string, int> resource in curDef.UpgradeCost)
                context.State.Player.GameBalance[resource.Key] -= resource.Value;
            ObjectState gradeState = new ObjectState();
            gradeState.CurrentState = Const.UpgradeStateId;
            gradeState.StateStartTime = Time;
            toUpgrade.State.Value = gradeState;
        }
    }
}
