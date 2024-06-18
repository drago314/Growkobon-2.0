using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDoorAnimator : MonoBehaviour
{
    private Animator animator;

    private TLWorldDoor door;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.Inst.OnMapEnter += OnMapEnter;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnMapEnter -= OnMapEnter;
        }
    }

    private void OnMapEnter(GameState gameState)
    {
        door = gameState.GetTLOfTypeAtPos<TLWorldDoor>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        InstantActivateDoor();
    }

    private void OnLevelComplete(string levelName)
    {
        if (door.JustOpened())
        {
            animator.ResetTrigger("InstantClose");
            animator.ResetTrigger("InstantOpen");
            animator.SetTrigger("OpenDoor");
            Debug.Log("Opening Door");  
        }
    }

    private void InstantActivateDoor()
    {
        if (door.IsOpen())
            animator.SetTrigger("InstantOpen");
        else
            animator.SetTrigger("InstantClose");
        Debug.Log("Instant Activating Door");
    }
}
