using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TLLevel : TLObject
{
    public string levelName;
    public bool unlocked;

    public TLLevel(Vector2Int pos, LevelSignature sig) : base(pos)
    {
        levelName = sig.levelName;
    }

    public TLLevel(TLLevel obj) : base(obj)
    {
        levelName = obj.levelName;
        unlocked = obj.unlocked;
    }
    
    public override string GetName() { return levelName; }
}
