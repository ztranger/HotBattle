using System;
using SharedLogic;
using SharedLogic.Actions;
using SharedLogic.Defs.Buildings;
using Tanat.SharedLogic.Defs;
using Tanat.SharedLogic.Defs.Buildings;
using Tanat.SharedLogic.GameState.Buildings;
using UnityEngine;

namespace Tanat.SharedLogic.Actions
{
    public class CollectResources : GameAction
    {
        private readonly string _buildingId;

        public CollectResources(string buildingId)
        {
            _buildingId = buildingId;
        }

        internal override void Execute(Context context)
        {
            MapItem toCollect = context.State.Location.Buildings[_buildingId].Value;

            if(toCollect.State.Value.CurrentState.Value != Const.IdleStateId)
                throw new Exception("cant collect resources not in idle state");

            FarmDef curDef = context.Defs.Buildings[toCollect.DefId] as FarmDef;
            long elapsedTime = (Time - toCollect.State.Value.StateStartTime.Value) / 1000;
            long resAmount = elapsedTime*curDef.ResPerSecond;
            int res = (int)Math.Min(resAmount, curDef.Capacity);
            if (context.State.Player.GameBalance.ContainsKey(curDef.ResourceId))
                context.State.Player.GameBalance[curDef.ResourceId] =
                    context.State.Player.GameBalance[curDef.ResourceId] + res;
            else
                context.State.Player.GameBalance.Add(curDef.ResourceId, res);

            ObjectState gradeState = new ObjectState();
            gradeState.CurrentState = Const.IdleStateId;
            gradeState.StateStartTime = Time;
            toCollect.State.Value = gradeState;
        }
    }
}

