using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public int damage = 1;
    public float range = 2f;
    public LayerMask mask;

    [Header("UI Setup")]
    [SerializeField] private GameObject hpDisplayParent;
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Audio")]
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private AudioClip repairSound;

    private bool isPlayingLoop = false;

    private PlayerInput input;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        HandleHPDisplay();

        bool isHolding = input.actions.FindAction("Attack").IsPressed();

        if (isHolding)
        {
            TryHit();
        }
        else
        {
            StopLoopSound();
        }
    }

    void TryHit()
    {
        Item equipped = PlayerHotbarController.Instance.GetEquippedItem();
        if (equipped == null)
        {
            StopLoopSound();
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, mask))
        {
            // 🔧 REPAIR
            if (equipped.toolType == ToolType.Hammer)
            {
                BuildingID building = hit.collider.GetComponentInParent<BuildingID>();
                if (building != null)
                {
                    if (building.Health < building.MaxHealth)
                    {
                        building.Heal(equipped.repairPower * Time.deltaTime);
                        PlayLoopSound(repairSound);
                    }
                    else
                    {
                        StopLoopSound();
                    }
                    return;
                }
            }

            // ⛏️ BREAK
            BreakableObject breakable = hit.collider.GetComponentInParent<BreakableObject>();

            if (breakable != null)
            {
                if (CanBreak(equipped.toolType, breakable.breakType))
                {
                    breakable.TakeDamage(damage * Time.deltaTime);

                    // 🔊 Use object-specific sound
                    PlayLoopSound(breakable.breakLoopSound);

                    return;
                }
            }
        }

        // ❌ Nothing valid hit
        StopLoopSound();
    }

    bool CanBreak(ToolType tool, BreakType target)
    {
        if (tool == ToolType.Axe && target == BreakType.Tree)
            return true;

        if (tool == ToolType.Pickaxe && target == BreakType.Stone)
            return true;

        return false;
    }

    void PlayLoopSound(AudioClip clip)
    {
        if (loopSource == null || clip == null) return;

        if (isPlayingLoop && loopSource.clip == clip) return;

        loopSource.clip = clip;
        loopSource.loop = true;
        loopSource.Play();

        isPlayingLoop = true;
    }

    void StopLoopSound()
    {
        if (loopSource == null) return;

        loopSource.Stop();
        loopSource.clip = null;
        isPlayingLoop = false;
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