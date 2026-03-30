using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    InventoryToggle inventoryToggle;
    Builder builder;
    public string mainMenuSceneName = "MainMenu";

    public bool isPaused = false;

    void Start()
    {
        builder = GameObject.Find("Builder").GetComponent<Builder>();
        inventoryToggle = GameObject.Find("InventoryManager").GetComponent<InventoryToggle>();
        pauseMenuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        builder.building = false;
        inventoryToggle.CloseInventory();
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f; // important so the next scene isn't frozen
        SceneManager.LoadScene(mainMenuSceneName);
    }
}