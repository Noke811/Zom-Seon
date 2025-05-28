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
    public void DisplayItemButtons()
    {
        ItemType type = GameManager.Instance.Inventory.selectedItemType;
        bool isEquipable = type == ItemType.Equipable;
        bool isConsumable = type == ItemType.Consumalbe;
        bool canSetQuickslot = isEquipable || isConsumable;

        buttons[(int)ItemButtonType.Equip].gameObject.SetActive(isEquipable);
        buttons[(int)ItemButtonType.Unequip].gameObject.SetActive(false); // 요건 나중에 수정 필요
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
                // 장비 장착
                break;

            case ItemButtonType.Unequip:
                // 장비 해제
                break;

            case ItemButtonType.Eat:
                // 소비 아이템 사용
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
