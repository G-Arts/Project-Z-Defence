using UnityEngine;

public class Pistol : WeaponBase
{
    [Header("Tabanca Özel Ayarlarý")]
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask hitMask = ~0;
    [SerializeField] private Transform firePoint;

    public override void Shoot()
    {
        if (isReloading || Time.time < nextFireTime) return;

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

    private void PerformHybridShootRaycast()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 rayOrigin = mainCam.transform.position;
        Vector3 rayDirection = mainCam.transform.forward;

        Vector3 targetPoint;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit screenCenterHit, range, hitMask))
        {
            targetPoint = screenCenterHit.point;
        }
        else
        {
            targetPoint = rayOrigin + (rayDirection * range);
        }

        Transform shootOrigin = firePoint != null ? firePoint : transform;
        Vector3 directionToTarget = (targetPoint - shootOrigin.position).normalized;

        if (Physics.Raycast(shootOrigin.position, directionToTarget, out RaycastHit shotHit, range, hitMask))
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