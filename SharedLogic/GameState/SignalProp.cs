using System;
using JsonFx.Json;
using SharedLogic;

namespace Tanat.SharedLogic.GameState
{
    public class SignalProp<T> : IJsonSerializable, ICustomConverter
    {
        public event Action<T> Changed;
        private T _value;

        public SignalProp(T val)
        {
            _value = val;
        }

        public SignalProp()
        {
        }

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (Changed != null)
                    Context.Instance.Emit(() => Changed(_value));
            }
        }

        public static implicit operator T(SignalProp<T> c)
        {
            return c._value;
        }

        public static implicit operator SignalProp<T>(T c)
        {
            return new SignalProp<T>(c);
        }

        public override string ToString()
        {
            if (_value == null)
                return "null";
            return _value.ToString();
        }

        public void ReadJson(JsonReader value)
        {
            _value = (T)value.ReadObject(typeof(T));
        }

        public void WriteJson(JsonWriter writer)
        {
            if (_value == null)
            {
                writer.TextWriter.Write("null");
            }
            else
            {
                writer.TextWriter.Write(JsonWriter.Serialize(_value));
            }
        }

        public object TryConvert(object value, Type targetType)
        {
            if (value is T)
                _value = (T)value;
            else
            {
                _value = (T)Convert.ChangeType(value, GetType().GetGenericArguments()[0]);
            }
            return this;
        }
    }
}
