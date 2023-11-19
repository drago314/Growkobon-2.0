using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSignature : TLSignature
{
    [SerializeField] public string levelName;
    [SerializeField] public int levelNumber;
    [SerializeField] public SerializableDictionary<string, List<Vector2Int>> exitToPathsUnlocked;
}
