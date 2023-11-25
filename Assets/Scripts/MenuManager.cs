using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public InputActionReference OpenMenuLevel, OpenMenuMap, CloseMenu;
    [SerializeField] public GameObject pauseMenuUI;
    [SerializeField] public Button titleScreenButton, mapButton;
    private bool menuOpen;
    private bool inMap;

    private void Start()
    {
        OpenMenuLevel.action.performed += OpenLevelMenu;
        OpenMenuMap.action.performed += OpenMapMenu;
        CloseMenu.action.performed += OpenMenu;
    }

    private void OnDestroy()
    {
        OpenMenuLevel.action.performed -= OpenLevelMenu;
        OpenMenuMap.action.performed -= OpenMapMenu;
        CloseMenu.action.performed -= OpenMenu;
    }

    private void OpenMapMenu(InputAction.CallbackContext obj)
    {
        inMap = true;
        OpenMenu(obj);
    }

    private void OpenLevelMenu(InputAction.CallbackContext obj)
    {
        inMap = false;
        OpenMenu(obj);
    }

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        if (SceneManager.GetActiveScene().name.Contains("Title"))
            return;

        if (menuOpen)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        GameManager.Inst.inputManager.SwitchCurrentActionMap("Pause Menu");
        pauseMenuUI.SetActive(true);
        if (inMap)
        {
            titleScreenButton.gameObject.SetActive(true);
            mapButton.gameObject.SetActive(false);
        }
        else
        {
            titleScreenButton.gameObject.SetActive(false);
            mapButton.gameObject.SetActive(true);
        }
        menuOpen = true;
    }

    public void Resume()
    { 
        if (inMap)
            GameManager.Inst.inputManager.SwitchCurrentActionMap("World Map");
        else
            GameManager.Inst.inputManager.SwitchCurrentActionMap("Gameplay");

        pauseMenuUI.SetActive(false);
        menuOpen = false;
    }

    public void OpenMap()
    {
        Resume();
        GameManager.Inst.OpenMap(GameManager.Inst.currentWorld, GameManager.Inst.currentLevel);
    }

    public void OpenTitleScreen()
    {
        Resume();
        GameManager.Inst.OpenTitleScreen();
    }
}
