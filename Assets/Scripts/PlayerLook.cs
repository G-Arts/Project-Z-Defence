using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Hassasiyet Ayarlarý")]
    [SerializeField] private float mouseSensitivity = 15f;

    [Header("Referanslar")]
    [SerializeField] private Transform playerBody; // Karakterin ana Gövdesi (Parent)
    [SerializeField] private Transform GunHolder;

    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- PLAYERINPUTHANDLER TARAFINDAN ÇAÐRILAN GÝRDÝ METODU ---
    public void ProcessLook(Vector2 lookInput)
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Yukarý / Aþaðý bakýþ açýsý (X ekseni etrafýnda dönme - Kamera & GunHolder)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (GunHolder != null)
        {
            GunHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Saða / Sola dönme (Y ekseni etrafýnda dönme - Karakter Gövdesi)
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}