using System.Collections;
using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Zombi/Hedef Özellikleri (Her tür için ayrý ayarlanabilir)")]
    [SerializeField] private float maxHealth = 50f;         // Bu zombinin caný
    [SerializeField] private int goldReward = 10;           // Öldüðünde vereceði altýn
    [SerializeField] private float damageToCenter = 10f;    // Merkeze ulaþtýðýnda vereceði hasar

    private float currentHealth;

    [Header("Hit Effect Settings")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color;
        }
    }

    private void OnEnable()
    {
        // Havuzdan her çýktýðýnda bu türe özel can ile baþlar
        currentHealth = maxHealth;

        if (targetRenderer != null)
        {
            targetRenderer.material.color = originalColor;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Vurulma efektini tetikle
        if (targetRenderer != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private IEnumerator FlashRoutine()
    {
        targetRenderer.material.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        targetRenderer.material.color = originalColor;
    }

    private void Die()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            targetRenderer.material.color = originalColor;
        }

        // GameManager'a bu zombiye özel altýn ödülünü bildiriyoruz
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddGold(goldReward);
        }

        // Object Pooling uyumlu: Pasif yap
        gameObject.SetActive(false);
    }

    // Zombi merkeze ulaþtýðýnda bu fonksiyon çaðrýlacak ve kendi hasar miktarýný merkeze iþleyecek
    public void ReachCenter()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DamageBase(damageToCenter);
        }

        gameObject.SetActive(false); // Merkeze varýnca da havuz için pasif yap
    }
}