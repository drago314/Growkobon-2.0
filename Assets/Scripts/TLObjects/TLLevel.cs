using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TLLevel : TLObject
{
    public string levelName;
    public int levelNumber;
    public bool unlocked;

    public TLLevel(Vector2Int pos, string level) : base(pos)
    {
        levelName = level;
        for (int i = 0; i < level.Length - 5; i++)
        { 
            if (level.Substring(i, 5).Equals("Level"))
            {
                levelNumber = Int32.Parse(level.Substring(i + 6, level.Length - i - 6));
                break;
            }
        }
    }

    public TLLevel(TLLevel obj) : base(obj)
    {
        levelNumber = obj.levelNumber;
        levelName = obj.levelName;
        unlocked = obj.unlocked;
    }
    
    public override string GetName() { return levelName; }
}
