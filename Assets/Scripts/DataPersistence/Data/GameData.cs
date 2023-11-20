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
        levelsCompleted = new List<string>();
    }
}
