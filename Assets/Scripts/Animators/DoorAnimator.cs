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
        manager.OnMoveEnd += OnMoveEnd;
        manager.OnMoveBegin += OnMoveBegin;
        GameManager.Inst.movementManager.OnUndoEnd += OnUndoOrResetEnd;
        GameManager.Inst.movementManager.OnResetEnd += OnUndoOrResetEnd;
        GameManager.Inst.OnLevelEnter += OnLevelLoaded;
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
            GameManager.Inst.OnLevelEnter -= OnLevelLoaded;
        }
    }

    private void OnLevelLoaded()
    {
        door = GameManager.Inst.movementManager.currentState.GetDoorAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }

    private void OnMoveBegin()
    {
        door = GameManager.Inst.movementManager.currentState.GetDoorAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        doorOpen = door.IsOpen();
    }

    private void OnMoveEnd()
    {
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
        if (door.IsOpen())
            animator.SetTrigger("InstantOpen");
        else
            animator.SetTrigger("InstantClose");
    }
}
