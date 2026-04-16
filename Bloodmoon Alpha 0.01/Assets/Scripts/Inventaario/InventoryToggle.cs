using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryUI;       // Parent of all inventory elements
    public string excludeTag = "Hotbar"; // Tag for UI elements that should always stay visible
    public KeyCode toggleKey = KeyCode.I;
    PauseMenu pausemenu;
    Builder builder;
    private GameObject PHUD;

    private PlayerInput input;

    private bool isOpen;

    private bool readyToTogle = true;

    void Start()
    {
        input = GameObject.Find("Character").GetComponent<PlayerInput>();
        PHUD = GameObject.Find("PlayerHUD");
        builder = GameObject.Find("Builder").GetComponent<Builder>();
        pausemenu = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
        SetInventoryActive(false);
        LockCursor();
    }

    void Update()
    {
        if (input.actions.FindAction("TogleInventory").IsPressed() && readyToTogle)
        {
            readyToTogle = false;
            if (isOpen)
                CloseInventory();
            else
                OpenInventory();
        }
        else if(!input.actions.FindAction("TogleInventory").IsPressed())
        {
            readyToTogle = true;
        }
    }

    public void OpenInventory()
    {
        if (!pausemenu.isPaused)
        {
            builder.building = false;
            PHUD.GetComponentInChildren<PriceDisplay>().UpdatePriceDisplay(null, new List<int>());
            isOpen = true;
            SetInventoryActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f; // optional
            PauseMenu.IsPaused = true;
        }
    }

    public void CloseInventory()
    {
        isOpen = false;
        SetInventoryActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f; // optional
        PauseMenu.IsPaused = false;
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