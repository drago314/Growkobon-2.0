using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    TLPlant plant;

    private void Start()
    {
        MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
        manager.OnMoveBegin += OnMoveBegin;
        manager.OnPlantMove += OnPlantMove;
        plant = GameManager.Inst.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            MovementManager manager = GameManager.Inst.gameObject.GetComponent<MovementManager>();
            manager.OnMoveBegin -= OnMoveBegin;
            manager.OnPlantMove -= OnPlantMove;
        }
    }

    private void OnMoveBegin()
    {
        plant = GameManager.Inst.currentState.GetPlantAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }


    private void OnPlantMove(MoveAction move)
    {
        if (plant != move.TLObj)
            return;

        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    public void Instantiate()
    {
        animator.SetTrigger("Idle");
    }

    public void Grow(Vector2Int growDir)
    {
        print("hi");
        animator.SetTrigger("GrowUp");
    }
}
