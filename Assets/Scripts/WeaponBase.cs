using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Temel Bilgiler")]
    public string weaponName;
    public WeaponRarity rarity = WeaponRarity.Common;
    public float damage = 20f;
    public float fireRate = 0.15f;
    public float impactForce = 30f;

    [Header("Mermi & Þarjör Yönetimi")]
    public int magazineSize = 30;       // Þarjör kapasitesi
    public int currentAmmo;             // Þarjördeki anlýk mermi
    public int totalAmmoReserve;        // Toplam yedek mermi stoðu
    public bool hasInfiniteAmmo = false; // Sadece baþlangýç tabancasý için true olabilir

    protected float nextFireTime;

    // Nadirliðe göre renk döndüren yardýmcý özellik (UI ve Kutu rengi için)
    public Color GetRarityColor()
    {
        switch (rarity)
        {
            case WeaponRarity.Common: return Color.white;
            case WeaponRarity.Uncommon: return new Color(0.2f, 0.6f, 1f); // Mavi
            case WeaponRarity.Rare: return new Color(1f, 0.2f, 0.2f);     // Kýrmýzý
            case WeaponRarity.Epic: return new Color(1f, 0.84f, 0f);     // Altýn
            case WeaponRarity.Legendary: return new Color(1f, 0.5f, 0f);  // Turuncu
            default: return Color.white;
        }
    }

    public virtual void Initialize()
    {
        currentAmmo = magazineSize;
    }

    public abstract void Shoot();

    // Þarjör deðiþtirme mekaniði
    public virtual void Reload()
    {
        // Þarjör zaten doluysa çýk
        if (currentAmmo >= magazineSize)
            return;

        if (hasInfiniteAmmo)
        {
            currentAmmo = magazineSize;

            Debug.Log($"{weaponName} yeniden dolduruldu. (Sonsuz yedek mermi)");
            return;
        }

        if (totalAmmoReserve <= 0)
            return;

        int bulletsNeeded = magazineSize - currentAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, totalAmmoReserve);

        currentAmmo += bulletsToReload;
        totalAmmoReserve -= bulletsToReload;

        Debug.Log($"{weaponName} þarjör deðiþtirildi. Kalan: {currentAmmo} / Yedek: {totalAmmoReserve}");
    }

    public virtual void Equip()
    {
        gameObject.SetActive(true);
    }

    public virtual void Unequip()
    {
        gameObject.SetActive(false);
    }
}