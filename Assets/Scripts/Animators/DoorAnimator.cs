using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer currentPotOverlay;
    [SerializeField] private SpriteRenderer requiredPotOverlay;

    [SerializeField] private Sprite[] currentPotOverlays;
    [SerializeField] private Sprite[] requiredPotOverlays;

    private Animator animator;

    private TLDoor door;
    private bool doorOpen = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        MovementManager manager = GameManager.Inst.movementManager;
        manager.OnMoveEnd += OnMoveEnd;
        manager.OnMoveBegin += OnMoveBegin;
        manager.OnUndoEnd += OnUndoOrResetEnd;
        manager.OnResetEnd += OnUndoOrResetEnd;
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
        InstantActivateDoor();
        UpdatePotCountOverlays();
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

        UpdatePotCountOverlays();
    }

    private void OnUndoOrResetEnd()
    {
        InstantActivateDoor();
        UpdatePotCountOverlays();
    }

    private void InstantActivateDoor()
    {
        if (door.IsOpen())
            animator.SetTrigger("InstantOpen");
        else
            animator.SetTrigger("InstantClose");
    }

    private void  UpdatePotCountOverlays()
    {
        if (!door.usesMultiPot)
        {
            currentPotOverlay.color = new Color(0f, 0f, 0f, 0f);
            requiredPotOverlay.color = new Color(0f, 0f, 0f, 0f);
            return;
        }

        var pots = GameManager.Inst.movementManager.currentState.GetAllTLPots();
        int potTotal = 0;
        foreach (var pot in pots)
        {
            potTotal += pot.IsFull();
        }
        currentPotOverlay.sprite = currentPotOverlays[potTotal];

        requiredPotOverlay.sprite = requiredPotOverlays[door.potsRequired];

        currentPotOverlay.color = new Color(255f, 255f, 255f, 1f);
        requiredPotOverlay.color = new Color(255f, 255f, 255f, 1f);
    }
}
