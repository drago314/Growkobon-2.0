using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{ 
    [SerializeField] private Sprite playerFacingUp;
    [SerializeField] private Sprite playerFacingLeft;
    [SerializeField] private Sprite playerFacingRight;
    [SerializeField] private Sprite playerFacingDown;

    private TLPlayer player;

    private void Start()
    {
        GameManager.Inst.OnMapEnter += OnLoad;
        GameManager.Inst.OnLevelEnter += OnLoad;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
        {
            GameManager.Inst.OnMapEnter -= OnLoad;
            GameManager.Inst.OnLevelEnter -= OnLoad;
        }
        if (player != null)
        {
            player.OnPlayerMove -= OnPlayerMove;
            player.OnUndoOrReset -= OnInstantMove;
        }
    }
        
    private void OnLoad(GameState gameState)
    {
        player = gameState.GetPlayer();
        player.OnPlayerMove += OnPlayerMove;
        player.OnUndoOrReset += OnInstantMove;

        gameObject.transform.position = new Vector3Int(player.GetPosition().x, player.GetPosition().y, 0);
        PlayerFaceDir(player.GetDirectionFacing());
    }

    private void OnPlayerMove(MoveAction move)
    {
        if (!player.IsObjectHeld())
            PlayerFaceDir(move.moveDir);
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    public void OnInstantMove(MoveAction move, InteractAction interact)
    {
        PlayerFaceDir(move.moveDir);
        transform.position = new Vector3(move.endPos.x, move.endPos.y, 0);
    }

    public void PlayerFaceDir(Vector2Int faceDir)
    {
        SpriteRenderer playerRenderer = GetComponent<SpriteRenderer>();

        if (faceDir == Vector2Int.up)
        {
            playerRenderer.sprite = playerFacingUp;
        }
        else if (faceDir == Vector2Int.down)
        {
            playerRenderer.sprite = playerFacingDown;
        }
        else if (faceDir == Vector2Int.left)
        {
            playerRenderer.sprite = playerFacingLeft;
        }
        else if (faceDir == Vector2Int.right)
        {
            playerRenderer.sprite = playerFacingRight;
        }
    }
}
