using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSignature : MoveableObjectSignature
{
    [SerializeField] public string levelName;
    [SerializeField] public int levelNumber;
    [SerializeField] public List<Vector2Int> pathsUnlocked;
}
