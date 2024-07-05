using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDoorAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer currentLevelsOverlay;
    [SerializeField] private SpriteRenderer requiredLevelsOverlay;

    [SerializeField] private Sprite[] currentLevelsOverlays;
    [SerializeField] private Sprite[] requiredLevelsOverlays;

    private Animator animator;

    private TLWorldDoor door;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameManager.Inst.OnMapEnter += OnMapEnter;
        GameManager.Inst.mapManager.OnLevelCompleted += OnLevelComplete;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnMapEnter -= OnMapEnter;
            GameManager.Inst.mapManager.OnLevelCompleted -= OnLevelComplete;
        }
    }

    private void OnMapEnter(GameState gameState)
    {
        door = gameState.GetTLOfTypeAtPos<TLWorldDoor>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        InstantActivateDoor();
        UpdateLevelCountOverlays();
    }

    private void OnLevelComplete()
    {
        if (door.JustOpened())
        {
            animator.ResetTrigger("InstantClose");
            animator.ResetTrigger("InstantOpen");
            animator.SetTrigger("OpenDoor");
            Debug.Log("Opening Door");  
        }
        UpdateLevelCountOverlays();
    }

    private void InstantActivateDoor()
    {
        if (door.IsOpen())
            animator.SetTrigger("InstantOpen");
        else
            animator.SetTrigger("InstantClose");
        Debug.Log("Instant Activating Door");
    }

    private void UpdateLevelCountOverlays()
    {
        var levels = GameManager.Inst.currentState.GetAllOfTLType<TLLevel>();
        int levelsTotal = 0;
        foreach (var level in levels)
        {
            if (level.IsCompleted())
                levelsTotal += 1;
        }

        currentLevelsOverlay.sprite = currentLevelsOverlays[levelsTotal];
        requiredLevelsOverlay.sprite = requiredLevelsOverlays[door.GetLevelsRequired()];
    }
}
