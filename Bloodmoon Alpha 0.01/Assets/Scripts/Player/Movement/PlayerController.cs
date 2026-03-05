using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : IDamageable
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float runMultiplier = 2.5f;
    public float crouchMultiplier = 0.5f;

    [Header("Jump")]
    public float jumpPower = 250f;

    [Header("Ground Check")]
    public string[] groundTags = { "Ground", "Floor" , "Stairs", "StairsRight", "StairsLeft", "StairsLoop" };

    [Header("Animation Params")]
    public string walkBool = "IsWalking";
    public string runBool = "IsRunning";
    public string groundedBool = "IsGrounded";
    public string jumpTrigger = "IsJumping";

    [Header("References")]
    [SerializeField] protected Slider healthBar;
    [SerializeField] protected Slider foodBar;
    [SerializeField] protected Slider waterBar;

    private Animator animator;
    private Rigidbody rb;

    public bool isOnGround = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (animator != null)
        {
            //animator.SetBool(groundedBool, true);
            animator.SetBool(walkBool, false);
            animator.SetBool(runBool, false);
        }
    }

    private void OnEnable()
    {
        if (healthBar == null)
            healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        if (foodBar == null)
            foodBar = GameObject.Find("FoodBar").GetComponent<Slider>();
        if (waterBar == null)
            waterBar = GameObject.Find("WaterBar").GetComponent<Slider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        HandleMovementAndAnimation();

        healthBar.value = health / maxHealth;
    }

    private void HandleMovementAndAnimation()
    {
        bool w = Input.GetKey(KeyCode.W);
        bool s = Input.GetKey(KeyCode.S);
        bool a = Input.GetKey(KeyCode.A);
        bool d = Input.GetKey(KeyCode.D);

        bool wantRun = Input.GetKey(KeyCode.LeftShift);
        bool wantCrouch = Input.GetKey(KeyCode.LeftControl);

        bool blockWalk = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.R);

        float multiplier = 1f;
        if (wantRun) multiplier = runMultiplier;
        if (wantCrouch) multiplier = crouchMultiplier; 

        Vector3 forward = transform.forward;
        Vector3 right = -transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = Vector3.zero;

        if (w) moveDir += forward;
        if (s) moveDir -= forward;
        if (a) moveDir += right;
        if (d) moveDir -= right;

        bool isMovingInput = moveDir.sqrMagnitude > 0.0001f;

        if (isMovingInput)
        {
            moveDir.Normalize();
            transform.position += moveDir * (moveSpeed * multiplier) * Time.deltaTime;
        }

        bool isWalking = isMovingInput && !blockWalk;
        bool isRunning = wantRun && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.R);

        if (animator != null)
        {
            animator.SetBool(walkBool, isWalking);
            animator.SetBool(runBool, isRunning);
        }
    }

    private void TryJump()
    {
        if (!isOnGround) return;

        isOnGround = false;
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);

        if (animator != null)
        {
            //animator.SetBool(groundedBool, false);
            animator.SetTrigger(jumpTrigger);
            animator.SetBool(runBool, false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsGroundObject(collision.gameObject))
        {
            isOnGround = true;
            //if (animator != null)
                //animator.SetBool(groundedBool, true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (IsGroundObject(collision.gameObject))
        {
            isOnGround = false;
            //if (animator != null)
                //animator.SetBool(groundedBool, false);
        }
    }

    private bool IsGroundObject(GameObject go)
    {
        for (int i = 0; i < groundTags.Length; i++)
        {
            if (go.CompareTag(groundTags[i]))
                return true;
        }
        return false;
    }
}