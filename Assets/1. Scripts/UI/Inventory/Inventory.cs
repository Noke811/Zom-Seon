using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventorySlot;
    InventorySlot[] slots;
    [SerializeField] int inventoryCount;
    [SerializeField] Transform dropPosition;

    public bool IsSwapMode { get; private set; }

    // 인벤토리 초기화
    public void Init()
    {
        slots = inventorySlot.GetComponentsInChildren<InventorySlot>(true);
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Init(i);
            slots[i].gameObject.SetActive(i < inventoryCount ? true : false);
        }
    }

    // 인벤토리에 아이템 추가
    public bool AddInventory(ItemData item, int amount)
    {
        int remain = amount;

        if (item.CanStack)
        {
            for (int i = 0; i < inventoryCount; i++)
            {
                if (slots[i].CanSave(item.Id))
                {
                    remain = slots[i].AddAmount(remain);
                    if(remain == 0) return true;
                }
            }
        }
        
        for (int i = 0; i < inventoryCount; i++)
        {
            if (slots[i].IsEmpty)
            {
                if (item.CanStack)
                {
                    remain = slots[i].SetSlot(item, remain);
                    if (remain == 0) return true;
                }

                slots[i].SetSlot(item);
                return true;
            }
        }

        // 인벤토리가 가득 차면 아이템을 인벤토리에 넣을 수 없음
        return false;
    }

    // 해당 슬롯 아이템 버리기
    public void DropItem(int index)
    {
        Instantiate(slots[index].GetDropItem(), dropPosition.position + dropPosition.forward, Quaternion.identity);
        slots[index].DecreaseAmount(1);
    }

    // 
    public void SwapItem(int index)
    {

    }
}
