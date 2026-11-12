using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Temel Bilgiler")]
    public string weaponName;
    public WeaponRarity rarity = WeaponRarity.Common;
    public Sprite weaponIcon;
    public float damage = 20f;
    public float fireRate = 0.15f;
    public float impactForce = 30f;
    public float range = 50f;
    public bool isAutomatic = false;

    [Header("Mermi & Þarjör Yönetimi")]
    public int magazineSize = 30;
    public int currentAmmo;
    public int totalAmmoReserve;
    public float reloadTime = 2.0f;     // Þarjör deðiþtirme süresi
    public bool hasInfiniteAmmo = false;

    public bool isReloading { get; protected set; } = false; // Þarjör dolduruluyor mu?
    private Coroutine reloadCoroutine;

    protected float nextFireTime;

    [HideInInspector] public PlayerInventory ownerInventory;

    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case WeaponRarity.Common: return Color.white;
            case WeaponRarity.Uncommon: return new Color(0.2f, 0.6f, 1f);
            case WeaponRarity.Rare: return new Color(1f, 0.2f, 0.2f);
            case WeaponRarity.Epic: return new Color(1f, 0.84f, 0f);
            case WeaponRarity.Legendary: return new Color(1f, 0.5f, 0f);
            default: return Color.white;
        }
    }

    public virtual void Initialize()
    {
        currentAmmo = magazineSize;
    }

    public abstract void Shoot();

    public virtual void Reload()
    {
        if (isReloading || currentAmmo >= magazineSize)
            return;

        if (!hasInfiniteAmmo && totalAmmoReserve <= 0)
            return;

        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }

        reloadCoroutine = StartCoroutine(ReloadRoutine());
    }

    protected virtual IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log($"<color=yellow>[{weaponName}]</color> Þarjör deðiþtiriliyor... ({reloadTime} sn)");

        yield return new WaitForSeconds(reloadTime);

        if (hasInfiniteAmmo)
        {
            currentAmmo = magazineSize;
        }
        else
        {
            int bulletsNeeded = magazineSize - currentAmmo;
            int bulletsToReload = Mathf.Min(bulletsNeeded, totalAmmoReserve);
            currentAmmo += bulletsToReload;
            totalAmmoReserve -= bulletsToReload;
        }

        isReloading = false;
        Debug.Log($"<color=green>[{weaponName}]</color> Þarjör yenilendi! Kalan: {currentAmmo} / {totalAmmoReserve}");

        if (ownerInventory != null)
        {
            ownerInventory.NotifyWeaponDataChanged();
        }
    }

    public virtual void Equip()
    {
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        // Silah býrakýldýðýnda þarjör doldurma iþlemini iptal et
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            isReloading = false;
        }
        gameObject.SetActive(false);
    }
}