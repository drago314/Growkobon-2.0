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
    private bool currentlyGrowing;

    private void Start()
    {
        MovementManager manager = GameManager.Inst.movementManager;
        manager.OnMoveEnd += OnMoveEnd;
        GameManager.Inst.OnLevelEnter += OnLevelLoaded;

        potOverlay = potOverlayChild.GetComponent<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            MovementManager manager = GameManager.Inst.movementManager;
            manager.OnMoveEnd -= OnMoveEnd;
            GameManager.Inst.OnLevelEnter -= OnLevelLoaded;
        }
        if (plant != null)
        {
            plant.OnPlantMove -= OnPlantMove;
            plant.OnUndoOrReset -= OnUndoOrReset;
            plant.OnPlantDeath -= UpdateDeadOrAlive;
            plant.OnPlantRegrowth -= UpdateDeadOrAlive;
            plant.OnPlantSkewered -= UpdateDeadOrAlive;
            plant.OnPlantUnskewered -= OnPlantSkewered;
            plant.OnPlantCornerSpinSheared -= OnPlantSkewered;
            plant.OnObjectDestroy -= DoneWithObject;
        }
    }

    private void OnLevelLoaded(GameState state)
    {
        plant = state.GetTLOfTypeAtPos<TLPlant>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        plant.OnPlantMove += OnPlantMove;
        plant.OnUndoOrReset += OnUndoOrReset;
        plant.OnPlantDeath += UpdateDeadOrAlive;
        plant.OnPlantRegrowth += UpdateDeadOrAlive;
        plant.OnPlantSkewered += UpdateDeadOrAlive;
        plant.OnPlantUnskewered += OnPlantSkewered;
        plant.OnPlantCornerSpinSheared += OnPlantSkewered;
        plant.OnObjectDestroy += DoneWithObject;

        UpdatePlantInPot();
        UpdateDeadOrAlive();
    }

    public void Instantiate(TLPlant plant)
    {
        this.plant = plant;
        plant.OnPlantMove += OnPlantMove;
        plant.OnUndoOrReset += OnUndoOrReset;
        plant.OnPlantDeath += UpdateDeadOrAlive;
        plant.OnPlantRegrowth += UpdateDeadOrAlive;
        plant.OnObjectDestroy += DoneWithObject;

        currentlyGrowing = true;
        UpdatePlantInPot();
        UpdateDeadOrAlive();
    }

    private void OnPlantMove(MoveAction move)
    {
        currentlyGrowing = false;
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    private void OnUndoOrReset(MoveAction move)
    {
        currentlyGrowing = false;
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
        UpdateDeadOrAlive();
    }

    public void Grow(Vector2Int growDir)
    {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
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
    }

    private void OnMoveEnd()
    {
        UpdatePlantInPot();
        UpdateDeadOrAlive();
    }

    private void UpdatePlantInPot()
    {
        potOverlay = potOverlayChild.GetComponent<SpriteRenderer>();
        plant = GameManager.Inst.currentState.GetTLOfTypeAtPos<TLPlant>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        if (GameManager.Inst.currentState.GetTLOfTypeAtPos<TLPot>(plant.GetPosition()) == null)
        {
            potOverlay.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            potOverlay.color = new Color(1f, 1f, 1f, 1f);
            potOverlay.sprite = potOverlays[0];
        }
    }

    private void OnPlantSkewered()
    {
        currentlyGrowing = false;
        UpdateDeadOrAlive();
    }

    private void UpdateDeadOrAlive()
    {
        if (plant.IsAlive())
            animator.SetBool("Dead", false);
        else
            animator.SetBool("Dead", true);

        if (!currentlyGrowing)
            animator.SetTrigger("Idle");
    }

    private void DoneWithObject()
    {
        Destroy(gameObject);
    }
}
