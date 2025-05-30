using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Info")]
    [SerializeField] GameObject inventorySlot;
    InventorySlot[] slots;
    [SerializeField] int inventoryCount;
    int selectedIndex;
    int equippedIndex;
    public ItemType selectedItemType => slots[selectedIndex].Data.Type;
    public bool IsSwapMode { get; private set; } // 인벤토리에서 아이템 이동 시에 사용

    [Header("Drop")]
    [SerializeField] Transform dropPosition;

    [Header("Quickslot")]
    [SerializeField] Transform quickslotUI;
    List<Image> quickslotImages = new List<Image>();
    int[] quickslot = new int[5];

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

        for (int i = 0; i < quickslotUI.childCount; i++)
        {
            Image icon = quickslotUI.GetChild(i).GetComponentsInChildren<Image>()[1];
            icon.sprite = null;
            icon.enabled = false;

            quickslotImages.Add(icon);
        }

        for (int i = 0; i < quickslot.Length; i++)
        {
            quickslot[i] = -1;
        }

        equippedIndex = -1;
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
        if(slots[selectedIndex].Data.Type == ItemType.Equipable)
            UnequipItem();

        Instantiate(slots[selectedIndex].Data.DropPrefab, dropPosition.position + dropPosition.forward, Quaternion.identity);
        slots[selectedIndex].DecreaseAmount(1);

        if (slots[selectedIndex].IsEmpty)
            ReleaseQuickslot();
    }

    // 해당 아이템 먹기
    public void EatItem()
    {
        // 아이템 효과 적용

        slots[selectedIndex].DecreaseAmount(1);
        if (slots[selectedIndex].IsEmpty) ReleaseQuickslot();
    }

    // 스왑 모드 활성화
    public void ActiveSwapMode()
    {
        IsSwapMode = true;
        GameManager.Instance.UIManager.PlayingUI.ItemButton.HideButtons();
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
            int tempQuickIndex = slots[selectedIndex].QuickIndex;

            if (slots[index].IsEmpty)
            {
                slots[selectedIndex].ClearSlot();
            }
            else
            {
                slots[selectedIndex].SetSlot(slots[index].Data, slots[index].Amount, slots[index].QuickIndex);
            }
            slots[index].SetSlot(tempData, tempAmount, tempQuickIndex);

            if(selectedIndex == equippedIndex) equippedIndex = index;

            IsSwapMode = false;
        }

        selectedIndex = index;

        if(selectedIndex == -1 || slots[selectedIndex].IsEmpty)
        {
            GameManager.Instance.UIManager.PlayingUI.ItemButton.HideButtons();
        }
        else
        {
            GameManager.Instance.UIManager.PlayingUI.ItemButton.DisplayItemButtons(selectedIndex == equippedIndex);
        }
    }

    // 퀵슬롯 저장
    public void SaveQuickslot(int quickNumber)
    {
        if (quickslot[quickNumber] == selectedIndex) return;

        // 다른 퀵슬롯에 저장되어 있었다면 해제
        ReleaseQuickslot();

        // 해당 퀵슬롯에 저장된 슬롯의 퀵슬롯 숫자 표기 지우기
        if(quickslot[quickNumber] != -1)
        {
            slots[quickslot[quickNumber]].SetQuickslotNum(-1);
        }

        // 퀵슬롯에 아이템 덮어씌우기
        quickslot[quickNumber] = selectedIndex;

        quickslotImages[quickNumber].sprite = slots[selectedIndex].Data.Icon;
        quickslotImages[quickNumber].SetNativeSize();
        quickslotImages[quickNumber].enabled = true;

        slots[selectedIndex].SetQuickslotNum(quickNumber + 1);
    }

    // 퀵슬롯 해제
    public void ReleaseQuickslot()
    {
        for (int i = 0; i < quickslot.Length; i++)
        {
            if (quickslot[i] == selectedIndex)
            {
                quickslot[i] = -1;

                quickslotImages[i].sprite = null;
                quickslotImages[i].enabled = false;

                slots[selectedIndex].SetQuickslotNum(-1);
                break;
            }
        }
    }

    // 퀵슬롯 선택
    public void SelectQuickslot(int quickNumber)
    {
        if (quickslot[quickNumber] == -1) return;
        selectedIndex = quickslot[quickNumber];

        if(slots[selectedIndex].Data.Type == ItemType.Equipable)
        {
            EquipItem();
        }
        else
        {
            EatItem();
        }
    }

    // 장비 장착하기
    public void EquipItem()
    {
        GameManager.Instance.Player.Equipment.Equip(slots[selectedIndex].Data.Id);
        // 장비 착용 시 스테이터스 오르는 로직 필요

        equippedIndex = selectedIndex;
        GameManager.Instance.UIManager.PlayingUI.ItemButton.DisplayItemButtons(selectedIndex == equippedIndex);
    }

    // 장비 해제하기
    public void UnequipItem()
    {
        GameManager.Instance.Player.Equipment.Unequip(slots[selectedIndex].Data.Id);
        equippedIndex = -1;
        GameManager.Instance.UIManager.PlayingUI.ItemButton.DisplayItemButtons(selectedIndex == equippedIndex);
    }

    // 해당 아이템 ID를 가진 자원의 수량 반환
    public int GetResourceAmount(int id)
    {
        int amount = 0;

        for (int i = 0; i < inventoryCount; i++)
        {
            if (!slots[i].IsEmpty)
            {
                if (slots[i].Data.Id == id)
                    amount += slots[i].Amount;
            }
        }

        return amount;
    }

    // 제작할 때 필요한 자원 소비
    public void CraftResource(int id, int amount)
    {
        int remain = amount;

        for (int i = 0; i < inventoryCount; i++)
        {
            if (!slots[i].IsEmpty)
            {
                if (slots[i].Data.Id == id)
                {
                    if(remain > slots[i].Amount)
                    {
                        remain -= slots[i].Amount;
                        slots[i].DecreaseAmount(slots[i].Amount);
                    }
                    else
                    {
                        slots[i].DecreaseAmount(remain);
                        return;
                    }
                }
            }
        }
    }
}
