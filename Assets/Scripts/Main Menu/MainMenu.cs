using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueGameButton;

    private void Start()
    {
        if (!DataPersistenceManager.instance.HasGameData())
        {
            continueGameButton.interactable = false;
        }
    }

    public void OnNewGameClicked()
    {
        DisableAllButtons();
        DataPersistenceManager.instance.NewGame();
        GameManager.Inst.OpenNewMap("World 1 Map");
    }

    public void OnContinueGameClicked()
    {
        DisableAllButtons();
        GameManager.Inst.OpenNewMap(GameManager.Inst.currentWorld);
    }

    private void DisableAllButtons()
    {
        newGameButton.interactable = false;
        continueGameButton.interactable = false;
    }
}
