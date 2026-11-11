using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    [Header("Envanter Ayarları")]
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private Transform dropPoint;

    [Header("Başlangıç Silahı (Opsiyonel)")]
    [SerializeField] private WeaponData startingWeaponData;

    private WeaponBase[] weaponSlots;
    private GameObject[] instantiatedWeaponObjects;
    private int activeSlotIndex = 0;

    public event Action<int> OnActiveSlotChanged;
    public event Action<int, string, Sprite> OnSlotDataChanged;

    private void Awake()
    {
        weaponSlots = new WeaponBase[maxSlots];
        instantiatedWeaponObjects = new GameObject[maxSlots];
    }

    private void Start()
    {
        if (startingWeaponData != null)
        {
            SpawnAndAddWeapon(startingWeaponData);
        }

        SwitchSlot(0);
    }

    // --- PLAYERINPUTHANDLER TARAFINDAN ÇAĞRILAN METOTLAR ---

    public WeaponBase GetActiveWeapon()
    {
        return weaponSlots[activeSlotIndex];
    }

    public void ExecuteShoot()
    {
        WeaponBase activeWeapon = GetActiveWeapon();
        if (activeWeapon != null)
        {
            activeWeapon.Shoot();
        }
    }

    public void ExecuteReload()
    {
        WeaponBase activeWeapon = GetActiveWeapon();
        if (activeWeapon != null)
        {
            activeWeapon.Reload();
        }
    }

    public void NextSlot()
    {
        int nextIndex = (activeSlotIndex + 1) % maxSlots;
        SwitchSlot(nextIndex);
    }

    public void PreviousSlot()
    {
        int prevIndex = (activeSlotIndex - 1 + maxSlots) % maxSlots;
        SwitchSlot(prevIndex);
    }

    public void SwitchSlot(int index)
    {
        if (index < 0 || index >= maxSlots) return;

        if (weaponSlots[activeSlotIndex] != null)
        {
            weaponSlots[activeSlotIndex].Unequip();
        }

        activeSlotIndex = index;

        if (weaponSlots[activeSlotIndex] != null)
        {
            weaponSlots[activeSlotIndex].Equip();
        }

        OnActiveSlotChanged?.Invoke(activeSlotIndex);
        NotifyWeaponDataChanged();
    }

    public bool SpawnAndAddWeapon(WeaponData data)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (weaponSlots[i] == null)
            {
                GameObject weaponObj = Instantiate(data.weaponPrefab, weaponHolderTransform, false);

                weaponObj.transform.localPosition = Vector3.zero;
                weaponObj.transform.localRotation = Quaternion.identity;
                weaponObj.transform.localScale = Vector3.one;

                WeaponBase weaponComponent = weaponObj.GetComponent<WeaponBase>();
                if (weaponComponent != null)
                {
                    weaponComponent.weaponName = data.weaponName;
                    weaponComponent.rarity = data.rarity;
                    weaponComponent.weaponIcon = data.weaponIcon;
                    weaponComponent.damage = data.damage;
                    weaponComponent.fireRate = data.fireRate;
                    weaponComponent.impactForce = data.impactForce;
                    weaponComponent.magazineSize = data.magazineSize;
                    weaponComponent.totalAmmoReserve = data.totalAmmoReserve;
                    weaponComponent.reloadTime = data.reloadTime;
                    weaponComponent.isAutomatic = data.isAutomatic;
                    weaponComponent.hasInfiniteAmmo = data.hasInfiniteAmmo;

                    weaponComponent.ownerInventory = this;
                    weaponComponent.Initialize();
                }

                weaponSlots[i] = weaponComponent;
                instantiatedWeaponObjects[i] = weaponObj;

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

    public void NotifyWeaponDataChanged()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            WeaponBase weapon = weaponSlots[i];
            if (weapon != null)
            {
                string ammoStr = weapon.hasInfiniteAmmo ? "∞" : $"{weapon.currentAmmo} / {weapon.totalAmmoReserve}";
                OnSlotDataChanged?.Invoke(i, ammoStr, weapon.weaponIcon);
            }
            else
            {
                OnSlotDataChanged?.Invoke(i, "", null);
            }
        }
    }
}