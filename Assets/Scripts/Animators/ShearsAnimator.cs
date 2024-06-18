using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShearsAnimator : MonoBehaviour
{
    TLShears shears;
    GameObject shearSprite;

    private void Start()
    {
        GameManager.Inst.OnLevelEnter += OnLevelLoaded;
        shearSprite = GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnLevelEnter -= OnLevelLoaded;
        }
        if (shears != null)
        {
            shears.OnMove -= OnShearsMove;
            shears.OnSpin -= OnShearsSpin;
        }
    }

    private void OnLevelLoaded(GameState state)
    {
        shears = GameManager.Inst.currentState.GetTLOfTypeAtPos<TLShears>(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        shears.OnMove += OnShearsMove;
        shears.OnSpin += OnShearsSpin;
    }

    private void OnShearsMove(MoveAction move)
    {
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    private void OnShearsSpin(SpinAction spin)
    {
        transform.position = new Vector3(spin.endPos.x, spin.endPos.y, 0);
        if (spin.clockwise)
            shearSprite.transform.eulerAngles = new Vector3(shearSprite.transform.eulerAngles.x, shearSprite.transform.eulerAngles.y, shearSprite.transform.eulerAngles.z - 90);
        else
            shearSprite.transform.eulerAngles = new Vector3(shearSprite.transform.eulerAngles.x, shearSprite.transform.eulerAngles.y, shearSprite.transform.eulerAngles.z + 90);
    }
}
