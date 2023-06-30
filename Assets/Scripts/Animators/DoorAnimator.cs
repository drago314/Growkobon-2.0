using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    TLDoor door;

    bool doorOpen = false;

    private void Start()
    {
        MovementManager manager = GameManager.Inst.movementManager;
        door = GameManager.Inst.movementManager.currentState.GetDoorAtPos(new Vector2Int((int) transform.position.x, (int) transform.position.y));
        manager.OnMoveEnd += OnMoveEnd;
        manager.OnMoveBegin += OnMoveBegin;
        GameManager.Inst.movementManager.OnUndoEnd += OnUndoOrResetEnd;
        GameManager.Inst.movementManager.OnResetEnd += OnUndoOrResetEnd;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            MovementManager manager = GameManager.Inst.movementManager;
            manager.OnMoveBegin -= OnMoveBegin;
            manager.OnMoveEnd -= OnMoveEnd;
            GameManager.Inst.movementManager.OnUndoEnd -= OnUndoOrResetEnd;
            GameManager.Inst.movementManager.OnResetEnd -= OnUndoOrResetEnd;
        }
    }

    private void OnMoveBegin()
    {
        doorOpen = door.IsOpen();
    }

    private void OnMoveEnd()
    {
        GameManager.Inst.DEBUG("move end    ");
        if (door.IsOpen() != doorOpen)
        {
            if (door.IsOpen())
                animator.SetTrigger("OpenDoor");
            else
                animator.SetTrigger("CloseDoor");
        }
    }

    private void OnUndoOrResetEnd()
    {
        GameManager.Inst.DEBUG("undid or reset: " + door.IsOpen().ToString());
        if (door.IsOpen())
            animator.SetTrigger("InstantOpen");
        else
            animator.SetTrigger("InstantClose");
    }
}
