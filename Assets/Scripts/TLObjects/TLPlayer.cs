using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TLPlayer : TLObject
{
    public void Move(Vector2Int moveDir)
    {
        gameObject.transform.position = gameObject.transform.position + new Vector3(moveDir.x, moveDir.y, 0);
        Vector2Int curPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        GameManager.Inst.currentState.MoveRelative(this, moveDir);
    }
}
