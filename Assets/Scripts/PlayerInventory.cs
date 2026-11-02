using UnityEngine;
using UnityEngine.InputSystem; // Yeni input sistemi kütüphanesi

public class PlayerInventory : MonoBehaviour
{
    [Header("Envanter Ayarlarý")]
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private Transform dropPoint;

    private WeaponBase[] weaponSlots;
    private int activeSlotIndex = 0;

    private void Awake()
    {
        weaponSlots = new WeaponBase[maxSlots];
    }

    // 1. Yeni Input Sistemindeki Action'lar (Input Actions Asset) üzerinden tetiklenecek metodlar:

    public void OnFire(InputAction.CallbackContext context)
    {
        // Tuþa basýldýðý an (Performed)
        if (context.performed)
        {
            if (weaponSlots[activeSlotIndex] != null)
            {
                weaponSlots[activeSlotIndex].Shoot();
            }
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (weaponSlots[activeSlotIndex] != null)
            {
                weaponSlots[activeSlotIndex].Reload();
            }
        }
    }

    public void OnDropWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DropCurrentWeapon();
        }
    }

    // Slot deðiþtirme (1'den 5'e kadar olan tuþlar için)
    public void OnSelectSlot1(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(0); }
    public void OnSelectSlot2(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(1); }
    public void OnSelectSlot3(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(2); }
    public void OnSelectSlot4(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(3); }
    public void OnSelectSlot5(InputAction.CallbackContext context) { if (context.performed) SwitchSlot(4); }

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
            Debug.Log($"Silah Kuþanýldý: Slot {activeSlotIndex + 1} ({weaponSlots[activeSlotIndex].weaponName})");
        }
    }

    void DropCurrentWeapon()
    {
        WeaponBase weaponToDrop = weaponSlots[activeSlotIndex];
        if (weaponToDrop == null || weaponToDrop.hasInfiniteAmmo) return;

        weaponToDrop.transform.SetParent(null);
        weaponToDrop.transform.position = dropPoint.position;
        weaponToDrop.Unequip();

        weaponSlots[activeSlotIndex] = null;
        Debug.Log("Silah yere býrakýldý.");
    }

    public bool AddWeapon(WeaponBase newWeapon)
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = newWeapon;
                newWeapon.transform.SetParent(weaponHolderTransform);
                newWeapon.transform.localPosition = Vector3.zero;
                newWeapon.transform.localRotation = Quaternion.identity;
                newWeapon.Initialize();

                if (i == activeSlotIndex) newWeapon.Equip();
                else newWeapon.Unequip();

                return true;
            }
        }
        return false;
    }
}