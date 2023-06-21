using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimator : TLAnimator
{
    [SerializeField] Sprite closedDoor;
    [SerializeField] Sprite openDoor;
    [SerializeField] SpriteRenderer spriteRenderer;

    // Update is called once per frame
    void Update()  //TODO FIX
    {
        GameState state = GameManager.Inst.currentState;
        TLDoor door = state.GetDoorAtPos(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        if (door.IsOpen())
            spriteRenderer.sprite = openDoor;
        else
            spriteRenderer.sprite = closedDoor;
    }
}
