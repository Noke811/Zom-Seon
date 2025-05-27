using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBase : MonoBehaviour
{
    public GameObject slotPrefab;
    public List<ItemData> itemDatas;

    private List<Slot> createdSlots = new List<Slot>();

    void OnEnable()
    {
        CreateSlots();
    }

    void OnDisable()
    {
        foreach (var slot in createdSlots)
        {
            if (slot != null && slot.gameObject != null)
            {
                Destroy(slot.gameObject);
            }
        }
        createdSlots.Clear();
    }

    void CreateSlots()
    {
        if (slotPrefab == null)
        {
            return;
        }
        if (itemDatas == null)
        {
            return;
        }
        foreach (var itemData in itemDatas)
        {
            GameObject slotObject = Instantiate(slotPrefab, transform);
            Slot slot = slotObject.GetComponent<Slot>();
            if (slot != null)
            {
                slot.itemData = itemData;
                slot.parentSlotBase = this;
                slot.UpdateSlotUI();
                createdSlots.Add(slot);
            }
            else
            {
                
                    Destroy(slotObject);
                
            }
        }
    }

    public void OnSlotSelected(ItemData selectedItem)
    {
        Debug.Log("Slot selected");
    }
}
