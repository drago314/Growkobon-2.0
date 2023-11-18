using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TLLevel : TLObject
{
    public string levelName;
    public int levelNumber;
    public bool unlocked;
    public SerializableDictionary<string, List<Vector2Int>> exitToPathsUnlocked;

    public TLLevel(Vector2Int pos, LevelSignature sig) : base(pos)
    {
        levelName = sig.levelName;
        levelNumber = sig.levelNumber;
        exitToPathsUnlocked = sig.exitToPathsUnlocked;
    }

    public TLLevel(TLLevel obj) : base(obj)
    {
        levelNumber = obj.levelNumber;
        levelName = obj.levelName;
        unlocked = obj.unlocked;
        exitToPathsUnlocked = obj.exitToPathsUnlocked;
    }

    public override string GetName() { return levelName; }
}
