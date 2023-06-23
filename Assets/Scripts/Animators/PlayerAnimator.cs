using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{ 
    [SerializeField] private Sprite playerFacingUp;
    [SerializeField] private Sprite playerFacingLeft;
    [SerializeField] private Sprite playerFacingRight;
    [SerializeField] private Sprite playerFacingDown;

    private void Start()
    {
        GameManager.Inst.gameObject.GetComponent<MovementManager>().OnPlayerMove += OnPlayerMove;
    }

    private void OnDestroy()
    {
        if (GameManager.Inst != null)
            GameManager.Inst.gameObject.GetComponent<MovementManager>().OnPlayerMove -= OnPlayerMove;
    }

    private void OnPlayerMove(MoveAction move)
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
