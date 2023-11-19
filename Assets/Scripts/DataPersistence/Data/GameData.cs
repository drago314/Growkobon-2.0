using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string currentWorld;
    public SerializableDictionary<string, bool> levelsCompleted;

    public GameData()
    {
        levelsCompleted = new SerializableDictionary<string, bool>();
    }

    public string GetCurrentWorldName()
    {
        return "World " + currentWorld + " Map";
    }
}
