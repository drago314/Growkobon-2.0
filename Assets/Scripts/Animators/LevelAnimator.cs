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
        GameManager.Inst.mapManager.OnLevelComplete += CompleteLevel;
        GameManager.Inst.OnMapEnter += Instantiate;
    }
    

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.mapManager.OnLevelComplete -= CompleteLevel;
            GameManager.Inst.OnMapEnter -= Instantiate;
        }
    }

    public void Instantiate(GameState gameState)
    {
        level = gameState.GetLevelAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        if (GameManager.Inst.IsLevelComplete(level.levelName))
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlay;
        else if (level.unlocked)
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlay;
    }

    public void CompleteLevel(string levelName)
    {
        if (level.levelName == levelName)
            levelOverlayChild.GetComponent<SpriteRenderer>().sprite = completedLevelOverlay;
    }
}
