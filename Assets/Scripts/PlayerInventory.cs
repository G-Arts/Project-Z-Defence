using UnityEngine;
using UnityEngine.InputSystem; // Yeni Input Sistemi için
using System;

public class PlayerInventory : MonoBehaviour
{
    [Header("Envanter Ayarları")]
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private Transform dropPoint;

    [Header("Başlangıç Silahı (Opsiyonel)")]
    [SerializeField] private WeaponData startingWeaponData; // Inspector'dan başlangıç tabancası verisi atanır

    private WeaponBase[] weaponSlots;
    private GameObject[] instantiatedWeaponObjects; // Sahneye üretilen 3D modelleri hafızada tutmak için
    private int activeSlotIndex = 0;

    public event Action<int> OnActiveSlotChanged;
    public event Action<int, string, Sprite> OnSlotDataChanged;

    private void Awake()
    {
        weaponSlots = new WeaponBase[maxSlots];
        instantiatedWeaponObjects = new GameObject[maxSlots];
    }

    void Start()
    {
        // Eğer Inspector'dan başlangıç silahı verildiyse 1. slota otomatik ekle
        if (startingWeaponData != null)
        {
            SpawnAndAddWeapon(startingWeaponData);
        }

        SwitchSlot(0);
    }

    // WeaponData kullanarak silahı sahnede oluşturan ve envantere ekleyen metod
    public bool SpawnAndAddWeapon(WeaponData data)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (weaponSlots[i] == null)
            {
                // 1. Prefab'i sahnede instantiate et
                GameObject weaponObj = Instantiate(data.weaponPrefab, weaponHolderTransform);
                weaponObj.transform.localPosition = Vector3.zero;
                weaponObj.transform.localRotation = Quaternion.identity;

                // 2. Silahın üzerindeki WeaponBase bileşenini al
                WeaponBase weaponComponent = weaponObj.GetComponent<WeaponBase>();
                if (weaponComponent != null)
                {
                    // WeaponData'daki temel verileri silaha aktar
                    weaponComponent.weaponName = data.weaponName;
                    weaponComponent.rarity = data.rarity;
                    weaponComponent.damage = data.damage;
                    weaponComponent.fireRate = data.fireRate;
                    weaponComponent.impactForce = data.impactForce;
                    weaponComponent.magazineSize = data.magazineSize;
                    weaponComponent.totalAmmoReserve = data.totalAmmoReserve;
                    weaponComponent.hasInfiniteAmmo = data.hasInfiniteAmmo;

                    weaponComponent.ownerInventory = this;
                    weaponComponent.Initialize();
                }

                // Slotlara kaydet
                weaponSlots[i] = weaponComponent;
                instantiatedWeaponObjects[i] = weaponObj;

                // Eğer eklenen slot aktif slotsa göster, değilse gizle
                if (i == activeSlotIndex)
                {
                    weaponComponent.Equip();
                }
                else
                {
                    weaponComponent.Unequip();
                }

                NotifyWeaponDataChanged();
                return true;
            }
        }

        Debug.Log("Envanter tamamen dolu!");
        return false;
    }

    public void SwitchSlot(int index)
    {
        if (index < 0 || index >= maxSlots) return;

        // Eski silahı pasif yap (Elimiz boşalır veya eskisi gizlenir)
        if (weaponSlots[activeSlotIndex] != null)
        {
            weaponSlots[activeSlotIndex].Unequip();
        }

        activeSlotIndex = index;

        // Yeni slotta silah varsa aktif et, yoksa el boş kalır
        if (weaponSlots[activeSlotIndex] != null)
        {
            weaponSlots[activeSlotIndex].Equip();
        }
        else
        {
            Debug.Log("Boş slot seçildi. El boş.");
        }

        OnActiveSlotChanged?.Invoke(activeSlotIndex);
        NotifyWeaponDataChanged();
    }

    // --- YENİ INPUT SİSTEMİ BAĞLANTILARI ---

    public void OnScrollInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 scrollValue = context.ReadValue<Vector2>();
        if (scrollValue.y > 0f)
        {
            // Yukarı scroll: Sonraki slota geç
            int nextIndex = (activeSlotIndex + 1) % maxSlots;
            SwitchSlot(nextIndex);
        }
        else if (scrollValue.y < 0f)
        {
            // Aşağı scroll: Önceki slota geç
            int prevIndex = (activeSlotIndex - 1 + maxSlots) % maxSlots;
            SwitchSlot(prevIndex);
        }
    }

    public void OnSelectSlot1(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(0); }
    public void OnSelectSlot2(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(1); }
    public void OnSelectSlot3(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(2); }
    public void OnSelectSlot4(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(3); }
    public void OnSelectSlot5(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(4); }

    public void NotifyWeaponDataChanged()
    {
        WeaponBase currentWeapon = weaponSlots[activeSlotIndex];
        if (currentWeapon != null)
        {
            string ammoStr = currentWeapon.hasInfiniteAmmo ? "Sonsuz" : $"{currentWeapon.currentAmmo} / {currentWeapon.totalAmmoReserve}";
            OnSlotDataChanged?.Invoke(activeSlotIndex, ammoStr, null);
        }
        else
        {
            OnSlotDataChanged?.Invoke(activeSlotIndex, "", null); // Boş slot için yazı yok
        }
    }
}