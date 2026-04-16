using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [SerializeField] private Item weaponItem;

    private Camera cam;
    private float defaultFOV;
    private bool isAiming;

    private PlayerInput input;

    private void Start()
    {
        input = GameObject.Find("Character").GetComponent<PlayerInput>();
        cam = Camera.main;
        defaultFOV = cam.fieldOfView;
    }

    private void Update()
    {
        HandleAim();
    }

    void HandleAim()
    {
        if (weaponItem == null) return;

        // Right mouse = aim
        isAiming = input.actions.FindAction("AimDownSights").IsPressed();

        float targetFOV = isAiming ? weaponItem.aimFOV : defaultFOV;

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            targetFOV,
            Time.deltaTime * weaponItem.aimSpeed
        );
    }
}