using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventorySlot;
    InventorySlot[] slots;
    [SerializeField] int inventoryCount;

    private void Awake()
    {
        slots = inventorySlot.GetComponentsInChildren<InventorySlot>(true);
    }

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(i < inventoryCount ? true : false);
        }
    }

    public void AddInventory(ItemData item)
    {
        if (item.CanStack)
        {
            for (int i = 0; i < inventoryCount; i++)
            {
                if (slots[i].CanSave(item.Id))
                {
                    slots[i].AddAmount();
                    return;
                }
            }
        }
        
        for (int i = 0; i < inventoryCount; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].SetSlot(item);
                return;
            }
        }

        // 인벤토리가 가득 차면 아이템을 선택해서 버림
        Debug.Log("인벤토리가 꽉 참!");
    }
}
