using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Hassasiyet Ayarlarý")]
    [SerializeField] private float mouseSensitivity = 15f;

    [Header("Referanslar")]
    [SerializeField] private Transform playerBody; // Karakterin ana Gövdesi (Parent)
    [SerializeField] private Transform GunHolder;

    [Header("Input Ayarlarý (Unity 6)")]
    [SerializeField] private InputAction lookAction = new InputAction("Look", type: InputActionType.Value, expectedControlType: "Vector2", binding: "<Mouse>/delta");

    private float xRotation = 0f;

    private void OnEnable()
    {
        lookAction.Enable();
    }

    private void OnDisable()
    {
        lookAction.Disable();
    }

    private void Start()
    {
        // Fare imlecini ekranýn ortasýna kitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // New Input System ile fare hareketini (delta) alýyoruz
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // 1. Yukarý / Aþaðý bakýþ açýsý (X ekseni etrafýnda dönme - Sadece Kamera)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Kafanýn tamamen ters dönmesini engelle
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        GunHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 2. Saða / Sola dönme (Y ekseni etrafýnda dönme - Karakter Gövdesi)
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}