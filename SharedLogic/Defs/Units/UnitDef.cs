using System;

namespace SharedLogic.Defs.Units
{
    public class UnitDef
    {
        public UnitType UnitType { get; private set; }
        public string PrefabName { get; private set; }
        public string ShootEffect { get; private set; }
        public float Damage { get; private set; }
        public float Health { get; private set; }
        public float Speed { get; private set; }
        public float FireRange { get; private set; }
        public float ShootPerMinute { get; private set; }
        public float Accuracy { get; private set; }

        public override string ToString()
        {
            string s = "UnitType : " + UnitType + Environment.NewLine;
            s += "PrefabName : " + PrefabName + Environment.NewLine;
            s += "ShootEffect : " + ShootEffect + Environment.NewLine;
            s += "Damage : " + Damage + Environment.NewLine;
            s += "Accuracy : " + Accuracy + Environment.NewLine;
            s += "Health : " + Health;
            return s;
        }
    }
}
