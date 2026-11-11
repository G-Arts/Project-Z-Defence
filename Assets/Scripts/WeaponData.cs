using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Weapon System/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Temel Bilgiler")]
    public string weaponName;
    public WeaponRarity rarity;
    public Sprite weaponIcon;          // UI'da görünecek resim
    public GameObject weaponPrefab;    // Yere atýldýðýnda veya eline alýndýðýnda çýkacak 3D model

    [Header("Ýstatistikler")]
    public float damage = 20f;
    public float fireRate = 0.15f;
    public float impactForce = 30f;
    public float range = 50f;
    public bool isAutomatic = false;   // Otomatik / Yarý otomatik seçeneði

    [Header("Mermi & Þarjör Yönetimi")]
    public int magazineSize = 30;
    public int totalAmmoReserve = 90;
    public float reloadTime = 2.0f;     // Silahtan silaha deðiþen þarjör yenileme süresi (Saniye)
    public bool hasInfiniteAmmo = false;
}