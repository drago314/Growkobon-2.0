using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLDoorMultiExit : TLDoor
{
    public int exitNumber;
    public TLDoorMultiExit(Vector2Int curPos, MultiExitDoorSignature doorSig) : base(curPos, doorSig)
    {
        exitNumber = doorSig.exitNumber;
    }

    public TLDoorMultiExit(TLDoorMultiExit obj) : base(obj)
    {
        exitNumber = obj.exitNumber;
    }

}
