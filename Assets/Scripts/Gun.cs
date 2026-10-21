using UnityEngine;
using UnityEngine.InputSystem; // Unity 6 Input System kütüphanesi

public class Gun : MonoBehaviour
{
    [Header("Silah Ayarlarý")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float impactForce = 30f;

    [Header("Referanslar")]
    [SerializeField] private Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;

    [Header("Input Ayarlarý (Unity 6)")]
    // Inspector üzerinden tuþ atamasýný doðrudan seçebilirsiniz (Örn: <Mouse>/leftButton)
    [SerializeField] private InputAction shootAction = new InputAction("Shoot", type: InputActionType.Button, binding: "<Mouse>/leftButton");

    private float nextTimeToFire = 0f;

    private void OnEnable()
    {
        // Input eylemini aktif et
        shootAction.Enable();
    }

    private void OnDisable()
    {
        // Input eylemini devre dýþý býrak
        shootAction.Disable();
    }

    private void Update()
    {
        // Unity 6 Input System: Tuþa basýlý tutulup tutulmadýðýný IsPressed() ile kontrol ediyoruz
        if (shootAction.IsPressed() && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        // Namlu ateþ efektini çalýþtýr
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        RaycastHit hit;

        // Kameranýn merkezinden ileriye doðru Raycast at
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log($"Vurulan Obje: {hit.transform.name}");

            // Vurulan objede Target bileþeni var mý?
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // Vurulan objede Rigidbody varsa fiziksel tepki/itme uygula
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce, ForceMode.Impulse);
            }

            // Vurulma efektini (mermi izi/kývýlcým) oluþtur
            if (impactEffect != null)
            {
                GameObject impactGo = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGo, 0.1f);
            }
        }
    }
}