using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventorySlot;
    InventorySlot[] slots;
    [SerializeField] int inventoryCount;
    [SerializeField] Transform dropPosition;

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
    public void AddInventory(ItemData item, int amount)
    {
        int remain = amount;

        if (item.CanStack)
        {
            for (int i = 0; i < inventoryCount; i++)
            {
                if (slots[i].CanSave(item.Id))
                {
                    remain = slots[i].AddAmount(remain);
                    if(remain == 0) return;
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
                    if (remain == 0) return;
                }

                slots[i].SetSlot(item);
                return;
            }
        }

        // 인벤토리가 가득 차면 아이템을 선택해서 버림
        Debug.Log("인벤토리가 꽉 참!");
    }

    // 해당 슬롯 아이템 버리기
    public void DropItem(int index)
    {
        Instantiate(slots[index].GetDropItem(), dropPosition.position + dropPosition.forward, Quaternion.identity);
        slots[index].DecreaseAmount(1);
    }
}
