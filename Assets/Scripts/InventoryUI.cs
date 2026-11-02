using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public struct SlotUI
    {
        public Image slotBackground;
        public Image weaponIcon;
        public TextMeshProUGUI ammoText;
    }

    [Header("Baðlantýlar")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private SlotUI[] slots;

    [Header("Renk Ayarlarý")]
    [SerializeField] private Color normalColor = Color.gray;
    [SerializeField] private Color selectedColor = Color.yellow;

    private void OnEnable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnActiveSlotChanged += HandleSlotChanged;
            playerInventory.OnSlotDataChanged += HandleDataChanged;
        }
    }

    private void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.OnActiveSlotChanged -= HandleSlotChanged;
            playerInventory.OnSlotDataChanged -= HandleDataChanged;
        }
    }

    private void HandleSlotChanged(int activeIndex)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].slotBackground != null)
            {
                slots[i].slotBackground.color = (i == activeIndex) ? selectedColor : normalColor;
            }
        }
    }

    private void HandleDataChanged(int index, string ammoString, Sprite icon)
    {
        if (index < 0 || index >= slots.Length) return;

        if (slots[index].ammoText != null)
        {
            slots[index].ammoText.text = ammoString;
        }

        if (slots[index].weaponIcon != null)
        {
            if (icon != null)
            {
                slots[index].weaponIcon.gameObject.SetActive(true);
                slots[index].weaponIcon.sprite = icon;
            }
            else
            {
                slots[index].weaponIcon.gameObject.SetActive(false);
            }
        }
    }
}