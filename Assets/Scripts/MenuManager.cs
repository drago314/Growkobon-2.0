using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private InputActionReference OpenMenuAction, CloseMenuAction;
    [SerializeField] private GameObject pauseMenuUI, settingsUI, remapKeybindsUI;
    [SerializeField] private Button titleScreenButton, mapButton;
    private bool menuOpen;
    private bool inMap;

    private void Start()
    {
        OpenMenuAction.action.performed += OpenMenu;
        CloseMenuAction.action.performed += OpenMenu;
    }

    private void OnDestroy()
    {
        OpenMenuAction.action.performed -= OpenMenu;
        CloseMenuAction.action.performed -= OpenMenu;
    }

    private void OpenMenu(InputAction.CallbackContext obj)
    {
        if (SceneManager.GetActiveScene().name.Contains("Title"))
            return;

        inMap = GameManager.Inst.inMap;

        if (menuOpen)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        GameManager.Inst.inputManager.SwitchCurrentActionMap("Pause Menu");
        DeactivateAllMenus();
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
        GameManager.Inst.inputManager.SwitchCurrentActionMap("Gameplay");

        DeactivateAllMenus();
        menuOpen = false;
    }

    public void OpenSettings()
    {
        DeactivateAllMenus();
        settingsUI.SetActive(true);
    }

    public void CloseSettings()
    {
        DeactivateAllMenus();
        pauseMenuUI.SetActive(true);
    }

    public void OpenKeybindRemap()
    {
        DeactivateAllMenus();
        remapKeybindsUI.SetActive(true);
    }

    public void CloseKeybindRemap()
    {
        DeactivateAllMenus();
        settingsUI.SetActive(true);
    }

    private void DeactivateAllMenus()
    {
        settingsUI.SetActive(false);     
        pauseMenuUI.SetActive(false);
        remapKeybindsUI.SetActive(false);
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
