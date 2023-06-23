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
        MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
        door = GameManager.Inst.currentState.GetDoorAtPos(new Vector2Int((int) transform.position.x, (int) transform.position.y));
        manager.OnMoveEnd += OnMoveEnd;
        manager.OnMoveBegin += OnMoveBegin;
        GameManager.Inst.undo.action.performed += OnUndoOrReset;
        GameManager.Inst.reset.action.performed += OnUndoOrReset;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
            manager.OnMoveBegin -= OnMoveBegin;
            manager.OnMoveEnd -= OnMoveEnd;
            GameManager.Inst.undo.action.performed -= OnUndoOrReset;
            GameManager.Inst.reset.action.performed -= OnUndoOrReset;
        }
    }

    private void OnMoveBegin()
    {
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

    private void OnUndoOrReset(InputAction.CallbackContext obj)
    {
        if (door.IsOpen())
            animator.SetTrigger("InstantOpen");
        else
            animator.SetTrigger("InstantClose");
    }
}
