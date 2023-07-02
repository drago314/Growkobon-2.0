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
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
        }
    }

    public void Instantiate()
    {
        level = GameManager.Inst.mapManager.currentState.GetLevelAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        levelOverlayChild.GetComponent<SpriteRenderer>().sprite = unlockedLevelOverlays[level.levelNumber];
    }
}
