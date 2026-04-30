using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using TMPro;

/// <summary>
/// Handles player interactions with breakable objects (trees, rocks, etc.)
/// Supports multi-mesh objects using parent lookup
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public int damage = 1;    
    public float range = 2f;  
    public LayerMask mask;
    [Header("UI Setup")]
    [SerializeField] private GameObject hpDisplayParent;
    [SerializeField] private TextMeshProUGUI hpText;             

    private PlayerInput input;
    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        HandleHPDisplay();
        // Left mouse click
        if (input.actions.FindAction("Attack").IsPressed())
        {
            TryHit();
        }
    }

    void TryHit()
    {
        Item equipped = PlayerHotbarController.Instance.GetEquippedItem();
        if (equipped == null) return;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            if (equipped.toolType == ToolType.Hammer)
            {
                BuildingID building = hit.collider.GetComponentInParent<BuildingID>();
                if (building != null)
                {
                    if (building.Health < building.MaxHealth)
                    {
                        building.Heal(equipped.repairPower * Time.deltaTime);
                    }
                    return;
                }
            }

            BreakableObject breakable = hit.collider.GetComponentInParent<BreakableObject>();

            if (breakable != null)
            {
                if (CanBreak(equipped.toolType, breakable.breakType))
                {
                    breakable.TakeDamage(damage * Time.deltaTime);
                    return;
                }
            }
        }
    }

    bool CanBreak(ToolType tool, BreakType target)
    {
        if (tool == ToolType.Axe && target == BreakType.Tree)
            return true;

        if (tool == ToolType.Pickaxe && target == BreakType.Stone)
            return true;

        return false;
    }
    private void HandleHPDisplay()
    {
        Item equipped = PlayerHotbarController.Instance.GetEquippedItem();

        if (equipped == null || equipped.toolType != ToolType.Hammer)
        {
            hpDisplayParent.SetActive(false);
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            BuildingID building = hit.collider.GetComponentInParent<BuildingID>();

            if (building != null)
            {
                hpDisplayParent.SetActive(true);
                hpText.text = $"{(int)building.Health} / {(int)building.MaxHealth}";
                return;
            }
        }

        hpDisplayParent.SetActive(false);
    }
}