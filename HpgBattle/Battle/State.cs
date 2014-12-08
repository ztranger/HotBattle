using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HPG.Battle.Entities;
using SharedLogic.GameState;
using Tanat.SharedLogic;

namespace HPG.Battle
{
    public class State
    {
        public static State Instance { get; private set; }
        
        private GameState _state;
        private GameDefs _defs;
        private IEventHandler _eventHandler;

        private State(GameDefs defs, GameState state, IEventHandler handler)
        {
            _state = state;
            _defs = defs;
            _eventHandler = handler;
        }

        public static State Init(GameDefs defs, GameState desc, IEventHandler handler)
        {
            if (Instance != null)
                throw new Exception("already initialized!");
            Instance = new State(defs, desc, handler);
            return Instance;
        }
    }
}
