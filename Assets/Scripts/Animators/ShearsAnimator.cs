using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShearsAnimator : MonoBehaviour
{

    private void Start()
    {
        GameManager.Inst.gameObject.GetComponent<MovementManager>().OnShearsMove += OnShearsMove;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.gameObject.GetComponent<MovementManager>().OnShearsMove -= OnShearsMove;
        }
    }

    private void OnShearsMove(MoveAction move)
    {
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }
}
