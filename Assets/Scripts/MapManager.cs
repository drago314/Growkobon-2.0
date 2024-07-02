using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System;

public class MapManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, enter;
    public event Action OnLevelCompleted;

    private void Start()
    {
        moveUp.action.performed += MoveUp;
        moveDown.action.performed += MoveDown;
        moveRight.action.performed += MoveRight;
        moveLeft.action.performed += MoveLeft;
        enter.action.performed += EnterLevel;
    }

    private void OnDestroy()
    {
        moveUp.action.performed -= MoveUp;
        moveDown.action.performed -= MoveDown;
        moveRight.action.performed -= MoveRight;
        moveLeft.action.performed -= MoveLeft;
        enter.action.performed -= EnterLevel;
    }


    private void MoveUp(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.up);
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.down);
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.right);
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        Move(Vector2Int.left);
    }

    private void Move(Vector2Int moveDir)
    {
        //print(currentState.ToString());

        GameManager.Inst.currentState.GetPlayer().SetDirectionFacing(moveDir);

        bool canMove = GameManager.Inst.currentState.GetPlayer().CanMove(null, moveDir);

        if (canMove)
            GameManager.Inst.currentState.GetPlayer().Move(null, moveDir);
    }

    private void EnterLevel(InputAction.CallbackContext obj)
    {
        GameState currentState = GameManager.Inst.currentState;

        TLLevel level = currentState.GetTLOfTypeAtPos<TLLevel>(currentState.GetPlayer().GetPosition());
        if (level != null)
        {
            GameManager.Inst.OpenLevel(level.GetLevelName());
        }
    }

    public void CompleteLevel(string levelName)
    {
        GameState currentState = GameManager.Inst.currentState;

        var TLLevels = currentState.GetAllOfTLType<TLLevel>();
        foreach (var level in TLLevels)
        {
            if (level.GetLevelName() == levelName)
            {
                level.SetCompleted(true);
                OnLevelCompleted?.Invoke();
                return;
            }
        }
    }
};
