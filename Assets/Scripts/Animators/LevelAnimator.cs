using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] lockedLevelOverlays;
    [SerializeField] private Sprite[] unlockedLevelOverlays;
    [SerializeField] private Sprite[] completedLevelOverlays;
    [SerializeField] private GameObject levelOverlayChild;

    private TLLevel level;
    private SpriteRenderer levelOverlayRenderer;

    void Start()
    {
        levelOverlayRenderer = levelOverlayChild.GetComponent<SpriteRenderer>();
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
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlays[level.levelNumber];
        else if (level.unlocked)
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlays[level.levelNumber];
        else
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = lockedLevelOverlays[level.levelNumber];
    }

    public void UnlockLevel(Vector2Int pos)
    {
        if (new Vector2Int((int)transform.position.x, (int)transform.position.y).Equals(pos))
        {
            level = GameManager.Inst.mapManager.currentState.GetLevelAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            if (GameManager.Inst.IsLevelComplete(level.levelName))
                levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlays[level.levelNumber];
            else
                levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlays[level.levelNumber];
        }
    }

    public void SetLevelNumber(int lvlNumber)
    {
        levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlays[lvlNumber];
    }
}
