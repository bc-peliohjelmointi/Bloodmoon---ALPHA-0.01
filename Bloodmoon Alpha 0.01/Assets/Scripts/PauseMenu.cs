using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    InventoryToggle inventoryToggle;
    Builder builder;
    private GameObject PHUD;
    public string mainMenuSceneName = "MainMenu";

    public bool isPaused = false;

    public GameObject settingsPanel;
    public static bool IsPaused;

    private PlayerInput input;

    private bool readytopause = true;

    void Start()
    {
        input = GameObject.Find("Character").GetComponent<PlayerInput>();
        PHUD = GameObject.Find("PlayerHUD");
        builder = GameObject.Find("Builder").GetComponent<Builder>();
        inventoryToggle = GameObject.Find("InventoryManager").GetComponent<InventoryToggle>();

        pauseMenuCanvas.SetActive(false);
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (input.actions.FindAction("PauseButton").IsPressed() && readytopause)
        {
            readytopause = false;
            if (settingsPanel.activeSelf)
            {
                CloseSettings();
                return;
            }

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
        else if(!input.actions.FindAction("PauseButton").IsPressed()) 
        {
            readytopause = true;
        }
    }

    public void PauseGame()
    {
        builder.building = false;
        PHUD.GetComponentInChildren<PriceDisplay>().UpdatePriceDisplay(null,new List<int>());
        inventoryToggle.CloseInventory();
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsPaused = false;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // important so the next scene isn't frozen
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        pauseMenuCanvas.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuCanvas.SetActive(true);
    }
}