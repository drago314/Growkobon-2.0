using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlantAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite deadHedge;
    [SerializeField] private Sprite aliveHedge;
    [SerializeField] private List<Sprite> potOverlays;
    [SerializeField] private GameObject potOverlayChild;

    private TLPlant plant;
    private SpriteRenderer potOverlay;

    private void Start()
    {
        MovementManager manager = GameManager.Inst.movementManager;
        manager.OnMoveBegin += OnMoveBegin;
        manager.OnPlantMove += OnPlantMove;
        manager.OnMoveEnd += OnMoveEnd;
        GameManager.Inst.OnLevelEnter += OnLevelLoaded;
        potOverlay = potOverlayChild.GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            MovementManager manager = GameManager.Inst.movementManager;
            manager.OnMoveBegin -= OnMoveBegin;
            manager.OnPlantMove -= OnPlantMove;
            manager.OnMoveEnd -= OnMoveEnd;
            GameManager.Inst.OnLevelEnter -= OnLevelLoaded;
        }
    }

    public void Instantiate()
    {
        animator.SetTrigger("Idle");
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        UpdatePlantInPot();
        UpdateDeadOrAlive();
    }

    private void OnLevelLoaded()
    {
        plant = GameManager.Inst.movementManager.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        UpdatePlantInPot();
        UpdateDeadOrAlive();
    }

    private void OnMoveBegin()
    {
        plant = GameManager.Inst.movementManager.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }


    private void OnPlantMove(MoveAction move)
    {
        if (plant != move.TLObj)
            return;

        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }


    public void Grow(Vector2Int growDir)
    {
        StartCoroutine(GrowAsync(growDir));
    }
    public IEnumerator GrowAsync(Vector2Int growDir)
    {
        if (growDir == Vector2Int.up)
            animator.SetTrigger("GrowUp");
        else if (growDir == Vector2Int.right)
            animator.SetTrigger("GrowRIGHT");
        else if (growDir == Vector2Int.left)
            animator.SetTrigger("GrowLEFT");
        else if (growDir == Vector2Int.down)
            animator.SetTrigger("GrowDOWN");
        yield return new WaitForSeconds(5 / 60f);
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(15 / 60f);
        UpdatePlantInPot();
    }

    private void OnMoveEnd()
    {
        UpdatePlantInPot();
        UpdateDeadOrAlive();
    }

    private void UpdatePlantInPot()
    {
        potOverlay = potOverlayChild.GetComponent<SpriteRenderer>();
        plant = GameManager.Inst.movementManager.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        if (GameManager.Inst.movementManager.currentState.GetPotAtPos(plant.curPos) == null)
        {
            potOverlay.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            potOverlay.color = new Color(1f, 1f, 1f, 1f);
            potOverlay.sprite = potOverlays[0];
        }
    }

    private void UpdateDeadOrAlive()
    {
        if (plant.isDead)
            animator.SetBool("Dead", true);
        else
            animator.SetBool("Dead", false);
    }
}
