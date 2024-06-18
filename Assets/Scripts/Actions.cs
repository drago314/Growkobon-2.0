using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction
{
    public Vector2Int origPos;
    public Vector2Int endPos;
    public Vector2Int moveDir;
    public TLObject TLObj;
    public GameState state;

    public MoveAction(Vector2Int originalPos, Vector2Int endPosition, Vector2Int moveDirection, TLObject obj, GameState gameState)
    {
        origPos = originalPos;
        endPos = endPosition;
        moveDir = moveDirection;
        TLObj = obj;
        state = gameState;
    }
}

public class SpinAction
{
    public Vector2Int origPos;
    public Vector2Int endPos;
    public Vector2Int origDir;
    public Vector2Int endDir;
    public TLObject TLObj;
    public GameState state;

    public SpinAction(Vector2Int originalPos, Vector2Int endPosition, Vector2Int originalDirection, Vector2Int endDirection, TLObject obj, GameState gameState)
    {
        origPos = originalPos;
        endPos = endPosition;
        origDir = originalDirection;
        endDir = endDirection;
        TLObj = obj;
        state = gameState;
    }
}

public class InteractAction
{
    public Vector2Int grabDirection;
    public TLPlayer player;
    public TLHoldableObject objectGrabbed;
    public GameState state;

    public InteractAction(Vector2Int grabDirection, TLPlayer player, TLHoldableObject objectGrabbed, GameState gameState)
    {
        this.grabDirection = grabDirection;
        this.player = player;
        this.objectGrabbed = objectGrabbed;
        state = gameState;
    }
}

public class GrowAction
{
    public Vector2Int newPos;
    public Vector2Int moveDir;
    public TLObject TLObj;
    public GameState state;

    public GrowAction(Vector2Int newPosition, Vector2Int moveDirection, TLObject obj, GameState gameState)
    {
        newPos = newPosition;
        moveDir = moveDirection;
        TLObj = obj;
        state = gameState;
    }
}
