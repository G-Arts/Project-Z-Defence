using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Zemin Kontrolü")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("Enerji (Stamina) Ayarlarý")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float sprintStaminaThreshold = 10f;

    // Ýç Deðiþkenler (Girdiler PlayerInputHandler tarafýndan beslenir)
    private Vector2 moveInput;
    private bool isSprinting;
    private bool jumpRequested;

    private float currentSpeed;
    private float currentStamina;
    private bool isGrounded;
    private Vector3 velocity;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        currentStamina = maxStamina;
    }

    // --- PLAYERINPUTHANDLER TARAFINDAN ÇAÐRILAN GÝRDÝ METOTLARI ---
    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    public void RequestJump()
    {
        if (isGrounded)
        {
            jumpRequested = true;
        }
    }

    private void Update()
    {
        if (groundCheck == null) return;

        // 1. Zemin Kontrolü
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Enerji ve Koþma Mantýðý
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isSprinting && isMoving && isGrounded && currentStamina > sprintStaminaThreshold)
        {
            currentSpeed = sprintSpeed;
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0f) currentStamina = 0f;
        }
        else
        {
            currentSpeed = walkSpeed;

            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }

        // 3. Yatay Hareket
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 4. Zýplama Uygulama
        if (jumpRequested)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpRequested = false;
        }

        // 5. Yerçekimi Uygulama
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public float GetStaminaPercentage()
    {
        return currentStamina / maxStamina;
    }
}