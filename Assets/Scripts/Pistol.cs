using UnityEngine;

public class Pistol : WeaponBase
{
    [Header("Tabanca Özel Ayarlarý")]
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Transform firePoint;      // Silahýn namlu ucu

    public override void Shoot()
    {
        if (Time.time < nextFireTime) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Þarjör bitti! R tuþuna basarak þarjör deðiþtirin.");
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Debug.Log($"[TABANCA] Ateþlendi! Þarjördeki Mermi: {currentAmmo} / {magazineSize}");

        if (ownerInventory != null)
        {
            ownerInventory.NotifyWeaponDataChanged();
        }

        PerformHybridShootRaycast();
    }

    // --- PROFESYONEL HÝBRÝT RAYCAST (FPS & TPS Uyumlu) ---
    private void PerformHybridShootRaycast()
    {
        // 1. Adým: Kameranýn merkezinden (ekranýn ortasýndan) ileriye doðru ray atýyoruz
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 rayOrigin = mainCam.transform.position;
        Vector3 rayDirection = mainCam.transform.forward;

        Vector3 targetPoint;

        // Kameranýn gönderdiði ray bir þeye çarpýyor mu?
        RaycastHit screenCenterHit;
        if (Physics.Raycast(rayOrigin, rayDirection, out screenCenterHit, range, hitMask))
        {
            // Çarptýðý nokta bizim hedef noktamýzdýr
            targetPoint = screenCenterHit.point;
        }
        else
        {
            // Hiçbir þeye çarpmadýysa menzilin sonundaki noktayý hedef al
            targetPoint = rayOrigin + (rayDirection * range);
        }

        // 2. Adým: Mermiyi namlu ucundan (firePoint) baþlatýp, bulduðumuz hedef noktasýna doðru gönderiyoruz
        Transform shootOrigin = firePoint != null ? firePoint : transform;
        Vector3 directionToTarget = (targetPoint - shootOrigin.position).normalized;

        RaycastHit shotHit;
        if (Physics.Raycast(shootOrigin.position, directionToTarget, out shotHit, range, hitMask))
        {
            Debug.Log($"Vurulan Obje: {shotHit.collider.name}");

            Target enemyTarget = shotHit.collider.GetComponent<Target>();
            if (enemyTarget != null)
            {
                enemyTarget.TakeDamage(damage);
            }

            if (shotHit.rigidbody != null)
            {
                shotHit.rigidbody.AddForce(-shotHit.normal * impactForce, ForceMode.Impulse);
            }
        }
    }
}