using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShearsAnimator : MonoBehaviour
{
    TLShears shears;
    GameObject shearSprite;

    private void Start()
    {
        GameManager.Inst.OnLevelEnter += OnLevelLoaded;
        GameManager.Inst.movementManager.OnUndoEnd += OnShearsInsantMove;
        GameManager.Inst.movementManager.OnResetEnd += OnShearsInsantMove;
        shearSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnLevelEnter -= OnLevelLoaded;
            GameManager.Inst.movementManager.OnUndoEnd -= OnShearsInsantMove;
            GameManager.Inst.movementManager.OnResetEnd -= OnShearsInsantMove;
        }
        if (shears != null)
        {
            shears.OnShearsMove -= OnShearsMove;
            shears.OnShearsSpin -= OnShearsSpin;
        }
    }

    private void OnLevelLoaded(GameState state)
    {
        shears = GameManager.Inst.currentState.GetTLOfTypeAtPos<TLShears>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        shears.OnShearsMove += OnShearsMove;
        shears.OnShearsSpin += OnShearsSpin;

        OnShearsInsantMove();
    }

    private void OnShearsMove(MoveAction move)
    {
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    private void OnShearsInsantMove()
    {
        transform.position = new Vector3(shears.GetPosition().x, shears.GetPosition().y, 0);
        float angle = 0;
        if (shears.GetDirectionFacing() == Vector2Int.up)
            angle = 90;
        else if (shears.GetDirectionFacing() == Vector2Int.left)
            angle = 180;
        else if (shears.GetDirectionFacing() == Vector2Int.down)
            angle = 270;

        shearSprite.transform.eulerAngles = new Vector3(shearSprite.transform.eulerAngles.x, shearSprite.transform.eulerAngles.y, angle);
    }

    private void OnShearsSpin(SpinAction spin)
    {
        Debug.Log("On Shears Spin Called: " + spin.endPos);
        transform.position = new Vector3(spin.endPos.x, spin.endPos.y, 0);
        if (spin.clockwise)
            shearSprite.transform.eulerAngles = new Vector3(shearSprite.transform.eulerAngles.x, shearSprite.transform.eulerAngles.y, shearSprite.transform.eulerAngles.z - 90);
        else
            shearSprite.transform.eulerAngles = new Vector3(shearSprite.transform.eulerAngles.x, shearSprite.transform.eulerAngles.y, shearSprite.transform.eulerAngles.z + 90);
    }
}
