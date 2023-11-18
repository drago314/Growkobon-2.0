using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] public InputActionReference OpenMenuLevel, OpenMenuMap, CloseMenu;
    [SerializeField] public GameObject pauseMenuUI;

    private bool menuOpen;
    private string currentActionMap;

    private void Start()
    {
        OpenMenuLevel.action.performed += OpenMenu;
        OpenMenuMap.action.performed += OpenMenu;
        CloseMenu.action.performed += OpenMenu;
    }

    private void OnDestroy()
    {
        OpenMenuLevel.action.performed -= OpenMenu;
        OpenMenuMap.action.performed -= OpenMenu;
        CloseMenu.action.performed -= OpenMenu;
    }
    private void OpenMenu(InputAction.CallbackContext obj)
    {
        if (menuOpen)
            Resume();
        else
            Pause();
    }

    private void Pause()
    {
        currentActionMap = GameManager.Inst.inputManager.currentActionMap.name;
        GameManager.Inst.inputManager.SwitchCurrentActionMap("Pause Menu");
        pauseMenuUI.SetActive(true);
        menuOpen = true;
    }

    public void Resume()
    { 
        GameManager.Inst.inputManager.SwitchCurrentActionMap(currentActionMap);
        pauseMenuUI.SetActive(false);
        menuOpen = false;
    }

    public void OpenMap()
    {
        Resume();
        GameManager.Inst.OpenMap();
    }
}
