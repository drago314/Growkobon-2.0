using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSignature : TLSignature
{
    [SerializeField] public string worldToTravelTo;
    [SerializeField] public Vector2Int posToTravelTo;
    [SerializeField] public List<Vector2Int> pathsBeginningUnlocked;
}
