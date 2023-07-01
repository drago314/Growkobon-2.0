using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLLevel : TLObject
{
    public string levelName;
    public bool unlocked;

    public TLLevel(Vector2Int pos, string level) : base (pos)
    {
        levelName = level;
    }

    public TLLevel(TLLevel obj) : base(obj)
    {
        levelName = obj.levelName;
        unlocked = obj.unlocked;
    }

    public override string GetName() { return levelName; }
}
