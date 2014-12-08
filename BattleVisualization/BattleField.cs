using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleVisualization
{
    class BattleField
    {
        readonly List<Unit> _units = new List<Unit>();

        public BattleField()
        {
            _units.Add(new Unit());
        }

        public void Draw()
        {
            foreach (Unit unit in _units)
            {
                unit.Draw();
            }
        }
    }
}
