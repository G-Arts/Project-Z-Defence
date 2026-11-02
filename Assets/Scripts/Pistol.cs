using UnityEngine;

public class Pistol : WeaponBase
{
    [Header("Tabanca Özel Ayarlarý")]
    [SerializeField] private float range = 50f;          // Atýþ menzili
    [SerializeField] private LayerMask hitMask;        // Raycast'in çarpacaðý katmanlar (Zombiler vb.)
    [SerializeField] private Transform firePoint;      // Merminin çýktýðý namlu ucu

    // WeaponBase'den miras aldýðýmýz Shoot metodunu tabancaya göre özelleþtiriyoruz
    public override void Shoot()
    {
        // Ateþ hýzý (FireRate) kontrolü - Seri týklamalarda spam yapýlmasýný önler
        if (Time.time < nextFireTime) return;

        // Mermi kontrolü: Eðer sonsuz mermili deðilse ve þarjörde mermi kalmadýysa ateþ etme
        if (currentAmmo <= 0)
        {
            Debug.Log("Þarjör bitti! R tuþuna basarak þarjör deðiþtirin.");
            // Ýleride buraya "týk týk" boþ silah ses efekti eklenebilir
            return;
        }

        // Sonraki ateþ için zamaný güncelle
        nextFireTime = Time.time + fireRate;

        // Sonsuz mermili deðilse þarjörden 1 mermi düþ
        if (!hasInfiniteAmmo)
        {
            currentAmmo--;
        }

        Debug.Log($"[TABANCA] Ateþlendi! Þarjördeki Mermi: {currentAmmo} / {magazineSize}");

        // Görsel veya Fiziksel Vuruþ Kontrolü (Raycast)
        PerformShootRaycast();
    }

    private void PerformShootRaycast()
    {
        // Namlu veya kamera yönünden ileriye doðru ray gönderiyoruz
        Transform shootOrigin = firePoint != null ? firePoint : transform;

        RaycastHit hit;
        if (Physics.Raycast(shootOrigin.position, shootOrigin.forward, out hit, range, hitMask))
        {
            Debug.Log($"Vurulan Obje: {hit.collider.name}");

            // Çarpýlan objede Target bileþeni var mý kontrol et (Zombi mi?)
            Target enemyTarget = hit.collider.GetComponent<Target>();
            if (enemyTarget != null)
            {
                // Zombiye hasar ver
                enemyTarget.TakeDamage(damage);
            }

            // Vurulan objede Rigidbody varsa fiziksel tepki/itme uygula
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce, ForceMode.Impulse);
            }
        }
    }
}