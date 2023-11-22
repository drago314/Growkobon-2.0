using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string currentWorld;
    public List<string> levelsCompleted;

    public GameData()
    {
        currentWorld = "World 1 Map";
        levelsCompleted = new List<string>();
    }
}
