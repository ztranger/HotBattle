using System;
using System.Collections.Generic;
using SharedLogic.Defs.Units;
using Tanat.SharedLogic.Defs.Buildings;

namespace Tanat.SharedLogic
{
    public class GameDefs
    {
        public Dictionary<string, UnitDef> Heroes { get; private set; }
        public Dictionary<string, UnitDef> Mobs { get; private set; }
        public Dictionary<string, FormationDef> Formations { get; private set; }
        public Dictionary<string, MapItemDef> Buildings { get; private set; }

        public UnitDef GetMobDescription(string name)
        {
            return GetUnitDescription(name, Mobs);
        }

        public UnitDef GetHeroDescription(string name)
        {
            return GetUnitDescription(name, Heroes);
        }

        public FormationDef GetFormationDescription(string name)
        {
            return GetUnitDescription(name, Formations);
        }

        public T GetBuildingDescription<T>(string name) where T : MapItemDef
        {
            return GetUnitDescription(name, Buildings) as T;
        }

        private T GetUnitDescription<T>(string name, Dictionary<string, T> hashTable) where T : class 
        {
            if (hashTable == null)
                throw new Exception("Definitions should be initialized");
            if (!hashTable.ContainsKey(name))
                return null;
            return hashTable[name];
        }
    }
}
