using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLLevel2Exit : TLLevel
{
    public List<Vector2Int> unlockablePaths2;

    public TLLevel2Exit(Vector2Int pos, Level2ExitSignature sig) : base(pos, sig)
    {
        unlockablePaths2 = sig.pathsUnlocked2;
    }

    public TLLevel2Exit(TLLevel2Exit obj) : base (obj)
    {
        unlockablePaths2 = obj.unlockablePaths2;
    }
}
