using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CollectWater : MonoBehaviour
{
    public GameObject WaterCollectionMarker;
    PlayerInput Input;
    Inventory Inv;
    Item Water;
    bool WaterCollected = false;
    private void Start()
    {
        foreach (var item in GetComponent<SaveMyStuff>().items) 
        {
            if (item.name == "Water") 
            {
                Water = item;
                break;
            }
        }
        Inv = GetComponent<Inventory>();
        Input = GameObject.Find("Character").GetComponent<PlayerInput>();
    }
    void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 5f))
        {
            if (hit.transform.tag == "Water")
            {
                WaterCollectionMarker.active = true;
                if (Input.actions.FindAction("Interact").IsPressed())
                {
                    if (!WaterCollected) 
                    {
                        Inv.SpawnInventoryItem(Water);
                        WaterCollected=true;
                    }
                }
                else
                {
                    WaterCollected = false;
                }
            }
            else
            {
                WaterCollectionMarker.active = false;
            }
        }
        else
        {
            WaterCollectionMarker.active = false;
        }
    }
}
