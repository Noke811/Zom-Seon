using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject inventorySlot;
    InventorySlot[] slots;
    [SerializeField] int inventoryCount;
    [SerializeField] Transform dropPosition;

    int selectedIndex;

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

        IsSwapMode = false;
    }

    // 인벤토리 UI가 다시 활성화되었을 때의 초기화
    private void OnEnable()
    {
        IsSwapMode = false;
        ChangeSelectedSlot(-1);
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
    public void DropItem()
    {
        Instantiate(slots[selectedIndex].Data.DropPrefab, dropPosition.position + dropPosition.forward, Quaternion.identity);
        slots[selectedIndex].DecreaseAmount(1);
    }

    // 스왑 모드 활성화
    public void ActiveSwapMode()
    {
        IsSwapMode = true;
        GameManager.Instance.UIManager.ItemButton.HideButtons();
    }

    // 슬롯 버튼을 눌렀을 때 인덱스 변경
    public void ChangeSelectedSlot(int index)
    {
        if(selectedIndex == index) return;

        // 스왑 모드가 켜져있을 때는 서로의 데이터와 수량 바꾸기
        if (IsSwapMode)
        {
            ItemData tempData = slots[selectedIndex].Data;
            int tempAmount = slots[selectedIndex].Amount;

            if (slots[index].IsEmpty)
            {
                slots[selectedIndex].ClearSlot();
            }
            else
            {
                slots[selectedIndex].SetSlot(slots[index].Data, slots[index].Amount);
            }
            slots[index].SetSlot(tempData, tempAmount);

            IsSwapMode = false;
        }

        selectedIndex = index;

        if(selectedIndex == -1 || slots[selectedIndex].IsEmpty)
        {
            GameManager.Instance.UIManager.ItemButton.HideButtons();
        }
        else
        {
            GameManager.Instance.UIManager.ItemButton.DisplayItemButtons(slots[selectedIndex].Data.Type);
        }
    }
}
