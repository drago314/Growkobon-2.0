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
        Move(new Vector2Int(0, 1));
    }

    private void MoveDown(InputAction.CallbackContext obj)
    {
        Move(new Vector2Int(0, -1));
    }
    private void MoveRight(InputAction.CallbackContext obj)
    {
        Move(new Vector2Int(1, 0));
    }
    private void MoveLeft(InputAction.CallbackContext obj)
    {
        Move(new Vector2Int(-1, 0));
    }

    private void Move(Vector2Int moveDir)
    {
        GameState state = GameManager.Inst.currentState;
        TLPlayer player = this.GetComponent<TLPlayer>();
        Vector2Int curPos = state.GetPosOf(player);
        Vector2Int goalPos = curPos + moveDir;
        // 2
        if (state.GetWallAtPos(goalPos) != null)
            return;

        //6 
        //print("goal pos: " + goalPos);
        if (state.GetPlantAtPos(goalPos) != null)
        {
            TLPlant[] plantGroup = state.GetPlantGroupAtPos(goalPos);
            bool canMove = true;
            foreach (var plant in plantGroup)
            {
                if (state.GetWallAtPos(state.GetPosOf(plant) + moveDir) != null)
                {
                    canMove = false;
                    break;
                }
            }
            if (canMove)
            {
                foreach (var plant in plantGroup)
                {
                    plant.Move(moveDir);
                }
                player.Move(moveDir);   
            }

            Vector2Int desiredPlantGrowth = goalPos + moveDir;
            while (state.GetPlantAtPos(desiredPlantGrowth) != null)
            {
                desiredPlantGrowth += moveDir;
            }
            if (state.GetWallAtPos(desiredPlantGrowth) == null)
            {
                GameObject newPlant = Instantiate(GameManager.Inst.plantPrefab, new Vector3(desiredPlantGrowth.x, desiredPlantGrowth.y, 0), Quaternion.identity);
                state.AddObject(newPlant.GetComponent<TLObject>(), desiredPlantGrowth);
            }
        }
        else
        {
            player.Move(moveDir);
        }

        print(state.ToString());                                
    }
}
