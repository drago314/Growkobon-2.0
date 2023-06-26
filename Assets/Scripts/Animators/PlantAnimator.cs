using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlantAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
 
    TLPlant plant;

    private bool growing;

    private void Start()
    {
        MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
        manager.OnMoveBegin += OnMoveBegin;
        manager.OnPlantMove += OnPlantMove;
        manager.OnMoveEnd += OnMoveEnd;
        GameManager.Inst.OnResetEnd += AfterResetOrUndo;
        GameManager.Inst.OnUndoEnd += AfterResetOrUndo;
        plant = GameManager.Inst.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));

        if (!growing)
            Idle();
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
            manager.OnMoveBegin -= OnMoveBegin;
            manager.OnPlantMove -= OnPlantMove;
            manager.OnMoveEnd -= OnMoveEnd;
            GameManager.Inst.OnResetEnd -= AfterResetOrUndo;
            GameManager.Inst.OnUndoEnd -= AfterResetOrUndo;
        }
    }

    private void OnMoveBegin()
    {
        plant = GameManager.Inst.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }

    private void OnMoveEnd()
    {
        Idle();
    }

    private void AfterResetOrUndo()
    {
        Idle();
    }

    private void Idle()
    {
        if (GameManager.Inst.currentState.GetPotAtPos(plant.curPos) != null)
            animator.SetTrigger("InPot");
        else
            animator.SetTrigger("Idle");
    }

    private void OnPlantMove(MoveAction move)
    {
        if (plant != move.TLObj)
            return;

        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    public void Instantiate()
    {
        animator.SetTrigger("Idle");
    }

    public void Grow(Vector2Int growDir)
    {
        growing = true;
        if (growDir == Vector2Int.up)
            animator.SetTrigger("GrowUp");
        else if (growDir == Vector2Int.right)
            animator.SetTrigger("GrowRIGHT");
        else if (growDir == Vector2Int.left)
            animator.SetTrigger("GrowLEFT");
        else if (growDir == Vector2Int.down)
            animator.SetTrigger("GrowDOWN");
    }
}
