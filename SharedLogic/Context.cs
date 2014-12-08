using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using JsonFx.Json;
using SharedLogic.Actions;
using Tanat.SharedLogic;

namespace SharedLogic
{
    public class Context
    {
        public GameDefs Defs { get; private set; }
        public GameState.GameState State { get; private set; }

        public static Context Instance { get; private set; }

        public Context()
        {
            if (Instance != null)
                throw new InvalidOperationException("context instance is already constructed");
            Instance = this;
        }

        #region game events

        private readonly List<Action> _emitedEvents = new List<Action>();

        internal void Emit(Action eventAction)
        {
            _emitedEvents.Add(eventAction);
        }

        private void FireEmittedEvents()
        {
            _emitedEvents.ForEach(evtAct => evtAct());
        }

        #endregion

        public void InitDefs(string defs)
        {
            if (Defs != null)
                throw new InvalidOperationException("defs is already initialized");
            Defs = JsonReader.Deserialize<GameDefs>(defs, TypeCreator); 
        }

        public void InitState(string stateData)
        {
            if (Defs == null)
                throw new InvalidOperationException("defs is not initialized");

            State = null;
            _emitedEvents.Clear();
            JsonReaderSettings settings = new JsonReaderSettings();
            settings.CustomConverter = new CustomConverter();
            JsonReader reader = new JsonReader(stateData, settings);
            State = reader.Deserialize<GameState.GameState>(TypeCreator);
        }

        private class CustomConverter : ICustomConverter
        {
            public object TryConvert(object value, Type targetType)
            {
                object res = null;
                if (targetType.Name == "SignalProp`1")
                {
                    res = Activator.CreateInstance(targetType, Convert.ChangeType(value, targetType.GetGenericArguments()[0]));
                }
                return res;
            }
        }

        private Type TypeCreator(string s)
        {
            return Type.GetType(s);
        }

        public void ReinitDataModels()
        {
            State.SendInited();
        }

        private int GetStableHash(string s)
        {
            uint hash = 0;
            foreach (byte b in Encoding.Unicode.GetBytes(s))
            {
                hash += b;
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }

            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);

            return (int) hash;
        }

        public int GetStateHash()
        {
            StringBuilder builder = new StringBuilder();
            JsonWriter writer = new JsonWriter(builder);
            writer.Write(State);
            string state = builder.ToString();
            Debug.WriteLine("StateDef : " + state);
            return GetStableHash(state);
        }

        public void Execute(GameAction action)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (action.ID != State.NextCommandId)
                throw new ApplicationException("invalid action id");

            State.Update(action.Time);
            action.Execute(this);
            FireEmittedEvents();
        }
    }
}