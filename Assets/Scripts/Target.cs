using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Hit Effect Settings")]
    [SerializeField] private Renderer targetRenderer; // Nesnenin MeshRenderer bileþeni
    [SerializeField] private Color hitColor = Color.white; // Vurulduðunda alacaðý renk
    [SerializeField] private float flashDuration = 0.1f; // Beyaz kalma süresi

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        // Eðer Inspector'dan atanmadýysa otomatik bulmaya çalýþ
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        // Malzemenin orijinal rengini sakla (Material property block kullanmak materyal kopyalanmasýný önler)
        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color;
        }
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;

        // Havuzdan tekrar çýktýðýnda rengin orijinal olduðundan emin olalým
        if (targetRenderer != null)
        {
            targetRenderer.material.color = originalColor;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Vurulma efektini tetikle
        if (targetRenderer != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        // Doðrulama: Canýn 0'a ulaþýp ulaþmadýðýný kontrol et
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private IEnumerator FlashRoutine()
    {
        // Rengi vurulma rengine (beyaza) çevir
        targetRenderer.material.color = hitColor;

        // Belirtilen süre kadar bekle
        yield return new WaitForSeconds(flashDuration);

        // Orijinal rengine geri döndür
        targetRenderer.material.color = originalColor;
    }

    private void Die()
    {
        // Eðer nesne ölürken hala yanýp sönme coroutine'i çalýþýyorsa durdur
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            targetRenderer.material.color = originalColor;
        }

        // Object Pooling uyumlu: Destroy yerine objeyi kapatýyoruz (pasif yapýyoruz)
        gameObject.SetActive(false);
    }
}