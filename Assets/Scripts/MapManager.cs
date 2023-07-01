using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, enter;
    public event System.Action<MoveAction> OnPlayerMove;

    public GameState currentState;

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
        TLPlayer player = currentState.GetPlayer();
        Vector2Int curPos = currentState.GetPosOf(player);
        Vector2Int goalPos = curPos + moveDir;

        bool noLevelInFront = currentState.GetLevelAtPos(goalPos) == null;// || !currentState.GetLevelAtPos(goalPos).unlocked;
        bool noPathInFront = currentState.GetPathAtPos(goalPos) == null || !currentState.GetPathAtPos(goalPos).unlocked;
        if (noLevelInFront && noPathInFront)
        {
            OnPlayerMove?.Invoke(new MoveAction(curPos, curPos, moveDir, player, currentState));
            return;
        }

        OnPlayerMove?.Invoke(new MoveAction(curPos, curPos + moveDir, moveDir, player, currentState));
        currentState.MoveRelative(player, moveDir);
        // OnPlayerMove?.Invoke(new MoveAction());
    }

    private void EnterLevel(InputAction.CallbackContext obj)
    {
        TLLevel level = currentState.GetLevelAtPos(currentState.GetPlayer().curPos);
        if (level != null)
        {
            GameManager.Inst.OpenLevel(level.levelName);
        }
    }
};
