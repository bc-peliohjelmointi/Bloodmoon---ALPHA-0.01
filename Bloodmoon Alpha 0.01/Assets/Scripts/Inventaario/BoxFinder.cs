using UnityEngine;
using UnityEngine.UI;

public class BoxFinder : MonoBehaviour
{
    public GameObject MainInventory;
    PauseMenu Pausemenu;

    private void Start()
    {
        Pausemenu = GameObject.Find("PauseMenu").GetComponent<PauseMenu>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && MainInventory.active == false && !Pausemenu.isPaused)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 10f))
            {
                if (hit.transform.GetComponent<Storage>() != null)
                {
                    GameObject.Find("InventoryManager").GetComponent<InventoryToggle>().OpenInventory();
                    GameObject.Find("CraftingMenu").SetActive(false);
                    if (hit.transform.GetComponent<BoxOpener>() != null)
                    {
                        hit.transform.GetComponent<BoxOpener>().StartLoop();
                    }
                }
            }
        }
    }
}
