using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPG.Battle.Actions
{
    class RequestHero : BattleAction
    {
        public override void Execute(params int[] args)
        {
            if (args.Length < 3)
                throw new ArgumentException("1 - hero id, 2 - pos X, 3 - pos Y");
        }
    }
}
