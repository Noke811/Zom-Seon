using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Image icon;

    ItemData data = null;
    public bool IsEmpty => data == null;
    int amount;
    bool isFull => amount >= data.MaxAmount;

    private void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.enabled = false;
    }

    public void SetSlot(ItemData _data)
    {
        data = _data;
        amount = 1;

        icon.sprite = data.Icon;
        icon.SetNativeSize();
        icon.enabled = true;
    }

    public void AddAmount()
    {
        amount++;
    }

    public void ClearSlot()
    {
        data = null;
        amount = 0;

        icon.sprite = null;
        icon.enabled = false;
    }

    public bool CanSave(int id)
    {
        return !IsEmpty && (data.Id == id) && !isFull;
    }
}
