using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAnimator : MonoBehaviour
{
    [SerializeField] private Sprite unlockedLevelOverlay;
    [SerializeField] private Sprite completedLevelOverlay;
    [SerializeField] private GameObject levelOverlayChild;

    private TLLevel level;

    void Start()
    {
        GameManager.Inst.OnMapEnter += OnLevelLoaded;
    }
    

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnMapEnter -= OnLevelLoaded;
        }
        if (level != null)
        {
            level.OnCompletion -= CompleteLevel;
        }
    }

    public void OnLevelLoaded(GameState gameState)
    {
        level = gameState.GetTLOfTypeAtPos<TLLevel>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        level.OnCompletion += CompleteLevel;

        if (level.IsCompleted())
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlay;
        else
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlay;
    }

    public void CompleteLevel()
    {
        levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlay;
    }
}
