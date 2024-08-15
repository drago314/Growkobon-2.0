using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrimmersAnimator : MonoBehaviour
{
    TLTrimmers trimmers;
    GameObject trimmersSprite;

    private void Start()
    {
        GameManager.Inst.OnLevelEnter += OnLevelLoaded;
        GameManager.Inst.movementManager.OnUndoEnd += OnTrimmersInstantMove;
        GameManager.Inst.movementManager.OnResetEnd += OnTrimmersInstantMove;
        trimmersSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnLevelEnter -= OnLevelLoaded;
            GameManager.Inst.movementManager.OnUndoEnd -= OnTrimmersInstantMove;
            GameManager.Inst.movementManager.OnResetEnd -= OnTrimmersInstantMove;
        }
        if (trimmers != null)
        {
            trimmers.OnTrimmersMove -= OnTrimmersMove;
            trimmers.OnTrimmersSpin -= OnTrimmersSpin;
        }
    }

    private void OnLevelLoaded(GameState state)
    {
        trimmers = GameManager.Inst.currentState.GetTLOfTypeAtPos<TLTrimmers>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        trimmers.OnTrimmersMove += OnTrimmersMove;
        trimmers.OnTrimmersSpin += OnTrimmersSpin;

        OnTrimmersInstantMove();
    }

    private void OnTrimmersMove(MoveAction move)
    {
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    private void OnTrimmersInstantMove()
    {
        transform.position = new Vector3(trimmers.GetPosition().x, trimmers.GetPosition().y, 0);
        float angle = 0;
        if (trimmers.GetDirectionFacing() == Vector2Int.left)
            angle = 90;
        else if (trimmers.GetDirectionFacing() == Vector2Int.down)
            angle = 180;
        else if (trimmers.GetDirectionFacing() == Vector2Int.right)
            angle = 270;

        trimmersSprite.transform.eulerAngles = new Vector3(trimmersSprite.transform.eulerAngles.x, trimmersSprite.transform.eulerAngles.y, angle);
    }

    private void OnTrimmersSpin(SpinAction spin)
    {
        Debug.Log("On Trimmers Spin Called: " + spin.endPos);
        transform.position = new Vector3(spin.endPos.x, spin.endPos.y, 0);
        if (spin.clockwise)
            trimmersSprite.transform.eulerAngles = new Vector3(trimmersSprite.transform.eulerAngles.x, trimmersSprite.transform.eulerAngles.y, trimmersSprite.transform.eulerAngles.z - 90);
        else
            trimmersSprite.transform.eulerAngles = new Vector3(trimmersSprite.transform.eulerAngles.x, trimmersSprite.transform.eulerAngles.y, trimmersSprite.transform.eulerAngles.z + 90);
    }
}
