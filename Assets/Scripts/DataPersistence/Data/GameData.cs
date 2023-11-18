using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string currentWorld;
    public Vector2Int playerPosition;
    public SerializableDictionary<string, bool> levelsCompleted;

    public GameData()
    {
        playerPosition = new Vector2Int();
        levelsCompleted = new SerializableDictionary<string, bool>();
    }

    public bool IsLevelComplete(string levelName)
    {
        foreach (string key in levelsCompleted.Keys)
        {
            if (key.Contains(levelName))
                return levelsCompleted[key];       
        }

        // This level hasn't been attempted yet
        return false;
    }

    public string GetCurrentWorldName()
    {
        return "World " + currentWorld + " Map";
    }
}
