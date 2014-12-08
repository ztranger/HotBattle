using JsonFx.Json;
using UnityEngine;
using System.Collections;

internal class LogicRandom
{
    public int Seed { get; set; } 

    public LogicRandom()
    {
        Seed = 0;
    }

    LogicRandom(int seed)
    {
        Seed = seed;
    }

    internal int Next()
    {
        Seed = (Seed * 9301 + 49297) % 233280;
        return Seed;
    }

    internal int Next(int maxValue)
    {
        return Next() % maxValue;
    }

    internal int Next(int minValue, int maxValue)
    {
        return minValue + Next() % (maxValue - minValue);
    }

    /*static void Export(LogicRandom itemRef, JsonWriter writer)
    {
        writer.Write(itemRef.mSeed);
    }*/

    public static implicit operator LogicRandom(int seed)
    {
        return new LogicRandom(seed);
    }
}