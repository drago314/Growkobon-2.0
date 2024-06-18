using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShearsAnimator : MonoBehaviour
{
    TLShears shears;
    private void Start()
    {
        GameManager.Inst.OnLevelEnter += OnLevelLoaded;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnLevelEnter -= OnLevelLoaded;
        }
        if (shears != null)
        {
            shears.OnMove -= OnShearsMove;
        }
    }

    private void OnLevelLoaded(GameState state)
    {
        shears = GameManager.Inst.currentState.GetTLOfTypeAtPos<TLShears>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        shears.OnMove += OnShearsMove;
    }

    private void OnShearsMove(MoveAction move)
    {
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }
}
