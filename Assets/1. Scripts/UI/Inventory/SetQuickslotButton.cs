public class SetQuickslotButton : ButtonHandler
{
    protected override void OnButtonClick(int index)
    {
        if(index != 5)
        {
            GameManager.Instance.Inventory.SaveQuickslot(index);
        }
        else
        {
            GameManager.Instance.Inventory.ReleaseQuickslot();
        }
        GameManager.Instance.UIManager.PlayingUI.ItemButton.DisplayItemButtons();
    }
}
