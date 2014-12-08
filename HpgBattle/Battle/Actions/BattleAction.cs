using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPG.Battle.Actions
{
    internal abstract class BattleAction
    {
        public abstract void Execute(params int[] args);
    }
}
