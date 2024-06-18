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
    public bool clockwise;
    public TLObject TLObj;
    public GameState state;

    public SpinAction(Vector2Int originalPos, Vector2Int endPosition, Vector2Int originalDirection, Vector2Int endDirection, bool clockwise, TLObject obj, GameState gameState)
    {
        origPos = originalPos;
        endPos = endPosition;
        origDir = originalDirection;
        endDir = endDirection;
        this.clockwise = clockwise;
        TLObj = obj;
        state = gameState;
    }
}

public class InstantMoveRotatableObject
{
    public Vector2Int pos;
    public Vector2Int direction;
    public TLObject TLObj;
    public GameState state;

    public InstantMoveRotatableObject(Vector2Int pos, Vector2Int direction, TLObject obj, GameState gameState)
    {
        this.pos = pos;
        this.direction = direction;
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

public class SkewerAction
{
    public Vector2Int skewerDirection;
    public TLPlayer player;
    public TLShears shears;
    public TLPlant plant;
    public GameState state;

    public SkewerAction(Vector2Int skewerDirection, TLPlayer player, TLShears shears, TLPlant plant, GameState gameState)
    {
        this.skewerDirection = skewerDirection;
        this.player = player;
        this.shears = shears;
        this.plant = plant;
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
