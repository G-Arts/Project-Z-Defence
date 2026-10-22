using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    // Havuzdan her aktif edildiðinde (sahneye çýktýðýnda) caný yenile
    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        // Doðrulama: Canýn 0'a ulaþýp ulaþmadýðýný kontrol et
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // Object Pooling uyumlu: Destroy yerine objeyi kapatýyoruz (pasif yapýyoruz)
        gameObject.SetActive(false);
    }
}