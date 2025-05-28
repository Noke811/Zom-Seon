using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Image icon;
    Text amountTxt;
    Text quickTxt;
    Button button;

    public ItemData Data { get; private set; }
    public bool IsEmpty => Data == null;
    public int Amount { get; private set; }
    bool isFull => Amount >= Data.MaxAmount;

    int index;

    // 인덱스 번호 부여 및 슬롯 초기화
    public void Init(int _index)
    {
        index = _index;
        
        icon = GetComponentsInChildren<Image>(true)[1];
        amountTxt = GetComponentsInChildren<Text>(true)[0];
        quickTxt = GetComponentsInChildren<Text>(true)[1];
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GameManager.Instance.Inventory.ChangeSelectedSlot(index));

        ClearSlot();
    }

    // 아이템 설정
    public void SetSlot(ItemData data)
    {
        Data = data;
        Amount = 1;

        icon.sprite = Data.Icon;
        icon.SetNativeSize();
        icon.enabled = true;

        UpdateAmountText();
    }

    // 아이템 설정 : 최대 수량을 벗어날 경우 최대까지 채우고 남은 수량을 반환
    public int SetSlot(ItemData data, int _amount)
    {
        Data = data;

        int remain = 0;
        if(_amount > Data.MaxAmount)
        {
            Amount = Data.MaxAmount;
            remain = _amount - Data.MaxAmount;
        }
        else
        {
            Amount = _amount;
        }

        icon.sprite = Data.Icon;
        icon.SetNativeSize();
        icon.enabled = true;

        UpdateAmountText();

        return remain;
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        Data = null;
        Amount = 0;

        icon.sprite = null;
        icon.enabled = false;

        UpdateAmountText();
        quickTxt.gameObject.SetActive(false);

        GameManager.Instance.UIManager.ItemButton.HideButtons();
    }

    // 해당 슬롯에 더 저장할 수 있는지 확인
    public bool CanSave(int id)
    {
        return !IsEmpty && (Data.Id == id) && !isFull;
    }

    #region Amount
    // 아이템 양 추가 : 최대 수량을 벗어날 경우 최대까지 채우고 남은 수량을 반환
    public int AddAmount(int _amount)
    {
        int remain = 0;

        if(Amount + _amount > Data.MaxAmount)
        {
            remain = Amount + _amount - Data.MaxAmount;
            Amount = Data.MaxAmount;
        }
        else
        {
            Amount += _amount;
        }

        UpdateAmountText();

        return remain;
    }

    // 아이템 양 감소
    public void DecreaseAmount(int _amount)
    {
        Amount -= _amount;

        if(Amount == 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateAmountText();
        }
    }

    // 수량 텍스트 업데이트
    private void UpdateAmountText()
    {
        if(Amount > 1)
        {
            amountTxt.text = Amount.ToString();
            amountTxt.gameObject.SetActive(true);
        }
        else
        {
            amountTxt.gameObject.SetActive(false);
        }
    }
    #endregion
}
