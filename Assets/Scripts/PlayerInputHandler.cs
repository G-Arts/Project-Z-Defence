using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Bileþen Baðlantýlarý")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerLook look;
    [SerializeField] private PlayerInventory inventory;

    [Header("Input Ayarlarý")]
    [SerializeField] private float scrollSensitivityThreshold = 10f;

    private void Awake()
    {
        // Inspector'dan atanmadýysa ayný obje üzerindeki bileþenleri otomatik bulur
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (look == null) look = GetComponent<PlayerLook>();
        if (inventory == null) inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        // Ýleride oyun duraklatýldýðýnda veya menü açýldýðýnda burayý aktif edebilirsin:
        // if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;

        HandleMovementInput();
        HandleLookInput();
        HandleWeaponInput();
        HandleSlotInput();
    }

    private void HandleMovementInput()
    {
        if (movement == null) return;

        // WASD Hareketi
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            float x = 0f;
            float y = 0f;
            if (Keyboard.current.wKey.isPressed) y += 1f;
            if (Keyboard.current.sKey.isPressed) y -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            moveInput = new Vector2(x, y).normalized;
        }

        movement.SetMoveInput(moveInput);

        // Koþma (Left Shift)
        bool isSprinting = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        movement.SetSprinting(isSprinting);

        // Zýplama (Space)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            movement.RequestJump();
        }
    }

    private void HandleLookInput()
    {
        if (look == null || Mouse.current == null) return;

        Vector2 lookDelta = Mouse.current.delta.ReadValue();
        look.ProcessLook(lookDelta);
    }

    private void HandleWeaponInput()
    {
        if (inventory == null || Mouse.current == null) return;

        WeaponBase activeWeapon = inventory.GetActiveWeapon();
        if (activeWeapon != null)
        {
            // Otomatik silahlar basýlý tutmayý, yarý otomatikler týklamayý dinler
            bool wantsToShoot = activeWeapon.isAutomatic
                ? Mouse.current.leftButton.isPressed
                : Mouse.current.leftButton.wasPressedThisFrame;

            if (wantsToShoot)
            {
                inventory.ExecuteShoot();
            }
        }

        // Þarjör Yenileme (R Tuþu)
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            inventory.ExecuteReload();
        }
    }

    private void HandleSlotInput()
    {
        if (inventory == null) return;

        // Sayý Tuþlarý (1-5)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) inventory.SwitchSlot(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) inventory.SwitchSlot(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) inventory.SwitchSlot(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) inventory.SwitchSlot(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) inventory.SwitchSlot(4);
        }

        // Fare Scroll Tekerleði (Hassasiyet 0.1f yapýldý)
        if (Mouse.current != null)
        {
            float scrollY = Mouse.current.scroll.ReadValue().y;

            if (scrollY > 0.1f)
            {
                inventory.NextSlot();
            }
            else if (scrollY < -0.1f)
            {
                inventory.PreviousSlot();
            }
        }
    }
}