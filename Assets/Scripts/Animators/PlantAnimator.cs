using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlantAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<Sprite> potOverlays;
    [SerializeField] private GameObject potOverlayChild;

    private TLPlant plant;
    private SpriteRenderer potOverlay;
    private SpriteRenderer spriteRenderer;

    private bool growCalled = false;
    private bool instantiateCalled = false;
    double timer = 0;

   /* private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
    }*/

    private void Start()
    {
        MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
        manager.OnMoveBegin += OnMoveBegin;
        manager.OnPlantMove += OnPlantMove;
        manager.OnMoveEnd += UpdatePlantInPot;
        GameManager.Inst.movementManager.OnLevelEnter += OnLevelLoaded;
        GameManager.Inst.movementManager.OnLevelEnter += UpdatePlantInPot;
        potOverlay = potOverlayChild.GetComponent<SpriteRenderer>();

        //StartCoroutine(WaitTilGrown()); //TODO
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
            manager.OnMoveBegin -= OnMoveBegin;
            manager.OnPlantMove -= OnPlantMove;
            manager.OnMoveEnd -= UpdatePlantInPot;
            GameManager.Inst.movementManager.OnLevelEnter -= OnLevelLoaded;
            GameManager.Inst.movementManager.OnLevelEnter -= UpdatePlantInPot;
        }
    }

    private void OnLevelLoaded()
    {
        plant = GameManager.Inst.movementManager.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
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

    public void Instantiate()
    {
        instantiateCalled = true;
        animator.SetTrigger("Idle");
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    public void Grow(Vector2Int growDir)
    {
        growCalled = true;
        if (growDir == Vector2Int.up)
            animator.SetTrigger("GrowUp");
        else if (growDir == Vector2Int.right)
            animator.SetTrigger("GrowRIGHT");
        else if (growDir == Vector2Int.left)
            animator.SetTrigger("GrowLEFT");
        else if (growDir == Vector2Int.down)
            animator.SetTrigger("GrowDOWN");
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator WaitTilGrown()
    {
        // TODO, makes plants wait before showing up on level start which is sorta bad
        yield return new WaitUntil(ReadyToContinue);
        if (!growCalled && !instantiateCalled)
            Instantiate();
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        UpdatePlantInPot();
    }

    private bool ReadyToContinue()
    {
        timer += Time.deltaTime;
        return growCalled || instantiateCalled || timer > 0.1;
    }

    private void UpdatePlantInPot()
    {
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
}
