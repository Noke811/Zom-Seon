public class ItemButton : ButtonHandler
{
    private enum ItemButtonType
    {
        Equip,
        Unequip,
        Eat,
        Quick,
        Move,
        Drop,
    }

    // 아이템 타입에 따른 버튼 표시
    public void DisplayItemButtons(bool isEquippedItem = false)
    {
        ItemType type = GameManager.Instance.Inventory.selectedItemType;
        bool isEquipable = type == ItemType.Equipable;
        bool isConsumable = type == ItemType.Consumalbe;
        bool canSetQuickslot = isEquipable || isConsumable;

        buttons[(int)ItemButtonType.Equip].gameObject.SetActive(isEquipable && !isEquippedItem);
        buttons[(int)ItemButtonType.Unequip].gameObject.SetActive(isEquipable && isEquippedItem); // 요건 나중에 수정 필요
        buttons[(int)ItemButtonType.Eat].gameObject.SetActive(isConsumable);
        buttons[(int)ItemButtonType.Quick].gameObject.SetActive(canSetQuickslot);
        buttons[(int)ItemButtonType.Move].gameObject.SetActive(true);
        buttons[(int)ItemButtonType.Drop].gameObject.SetActive(true);

        GameManager.Instance.UIManager.SetQuickslotButton.HideButtons();
    }

    // 해당 버튼 클릭했을 때 실행
    protected override void OnButtonClick(int index)
    {
        switch ((ItemButtonType)index)
        {
            case ItemButtonType.Equip:
                GameManager.Instance.Inventory.EquipItem();
                break;

            case ItemButtonType.Unequip:
                GameManager.Instance.Inventory.UnequipItem();
                break;

            case ItemButtonType.Eat:
                GameManager.Instance.Inventory.EatItem();
                break;

            case ItemButtonType.Quick:
                HideButtons();
                GameManager.Instance.UIManager.SetQuickslotButton.ShowButtons();
                break;

            case ItemButtonType.Move:
                GameManager.Instance.Inventory.ActiveSwapMode();
                break;

            case ItemButtonType.Drop:
                GameManager.Instance.Inventory.DropItem();
                break;
        }
    }
}
