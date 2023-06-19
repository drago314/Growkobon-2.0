using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] public InputActionReference moveUp, moveDown, moveRight, moveLeft, undo, reset;

    public static GameManager Inst;

    public GameState initialGameState;

    private void Awake()
    {
        Inst = this; // TODO fix
    }

    private void Start()
    {
        var thing = FindObjectsByType<TLObject>(FindObjectsSortMode.None);
        initialGameState = new GameState(FindObjectsByType<TLObject>(FindObjectsSortMode.None));
    }

    public void DEBUG(string message)
    {
        print(message);
    }
}
