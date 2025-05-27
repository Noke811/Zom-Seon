using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public ItemData itemData;
    public Image slotImage;
    public TextMeshProUGUI slotText;
    public Button slotButton;
    public SlotBase parentSlotBase;

    void Start()
    {
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(OnSlotClicked);
        }
        UpdateSlotUI();
    }
    
    public void UpdateSlotUI()
    {
        if (slotImage != null)
        {
            slotImage.sprite = itemData.itemSprite;
            slotText.text = itemData.itemName;
        }
        else
        {
            slotImage.sprite = null;
            slotText.text = "";
        }
    }

    void OnSlotClicked()
    {
        if (parentSlotBase != null && itemData != null)
        {
            parentSlotBase.OnSlotSelected(itemData);
        }
        else
        {
            {
                Debug.Log("Parent slot base or item data is null");
            }
        }
    }
}
