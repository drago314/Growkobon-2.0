using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private void Start()
    {
        GameManager.Inst.moveUp.action.performed += MoveUp;
        GameManager.Inst.moveDown.action.performed += MoveDown;
        GameManager.Inst.moveRight.action.performed += MoveRight;
        GameManager.Inst.moveLeft.action.performed += MoveLeft;
    }

    private void MoveUp(InputAction.CallbackContext obj)
    {
        Move(new Vector2(0, 1));
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        Move(new Vector2(0, -1));
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        Move(new Vector2(1, 0));
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        Move(new Vector2(-1, 0));
    }

    private void Move(Vector2 moveDir)
    {
        gameObject.transform.position = gameObject.transform.position + new Vector3(moveDir.x, moveDir.y, 0);
    }
}
