using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;       // Parent of all inventory elements
    public string excludeTag = "Hotbar"; // Tag for UI elements that should always stay visible
    public KeyCode toggleKey = KeyCode.I;
    PauseMenu pausemenu;

    private bool isOpen;

    void Start()
    {
        pausemenu = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
        SetInventoryActive(false);
        LockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (isOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    public void OpenInventory()
    {
        if (!pausemenu.isPaused)
        {
            isOpen = true;
            SetInventoryActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f; // optional
        }
    }

    public void CloseInventory()
    {
        isOpen = false;
        SetInventoryActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f; // optional
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetInventoryActive(bool active)
    {
        // Toggle all children except those with the excludeTag
        foreach (Transform child in inventoryUI.transform)
        {
            if (!child.CompareTag(excludeTag))
                child.gameObject.SetActive(active);
            if (child.name == "Storage")
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}