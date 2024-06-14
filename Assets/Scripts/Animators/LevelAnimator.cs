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
        GameManager.Inst.animator.OnLevelUnlock += UnlockLevel;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.animator.OnLevelUnlock -= UnlockLevel;
        }
    }

    public void Instantiate()
    {
        level = GameManager.Inst.mapManager.currentState.GetLevelAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        if (GameManager.Inst.IsLevelComplete(level.levelName))
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlay;
        else if (level.unlocked)
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlay;
    }

    public void UnlockLevel(Vector2Int pos)
    {
        if (new Vector2Int((int)transform.position.x, (int)transform.position.y).Equals(pos))
        {
            level = GameManager.Inst.mapManager.currentState.GetLevelAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            if (GameManager.Inst.IsLevelComplete(level.levelName))
                levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlay;
            else
                levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlay;
        }
    }
}
