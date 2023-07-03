using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TLLevel : TLObject
{
    public string levelName;
    public int levelNumber;
    public bool unlocked;
    public List<Vector2Int> unlockablePaths;

    public TLLevel(Vector2Int pos, LevelSignature sig) : base(pos)
    {
        levelName = sig.levelName;
        levelNumber = sig.levelNumber;
        unlockablePaths = sig.pathsUnlocked;
    }

    public TLLevel(TLLevel obj) : base(obj)
    {
        levelNumber = obj.levelNumber;
        levelName = obj.levelName;
        unlocked = obj.unlocked;
        unlockablePaths = obj.unlockablePaths;
    }
    
    public override string GetName() { return levelName; }
}
