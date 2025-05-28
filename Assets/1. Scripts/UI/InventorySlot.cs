using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Image icon;
    Text amountTxt;
    Text quickTxt;
    Button button;

    ItemData data;
    public bool IsEmpty => data == null;
    int amount;
    bool isFull => amount >= data.MaxAmount;

    int index;

    // 인덱스 번호 부여
    public void Init(int _index)
    {
        index = _index;
        
        icon = GetComponentsInChildren<Image>(true)[1];
        amountTxt = GetComponentsInChildren<Text>(true)[0];
        quickTxt = GetComponentsInChildren<Text>(true)[1];
        button = GetComponent<Button>();
        button.onClick.AddListener(OnSlotButtonClick);

        ClearSlot();
    }

    // 아이템 설정
    public void SetSlot(ItemData _data)
    {
        data = _data;
        amount = 1;

        icon.sprite = data.Icon;
        icon.SetNativeSize();
        icon.enabled = true;

        UpdateAmountText();
    }

    // 아이템 설정 : 최대 수량을 벗어날 경우 최대까지 채우고 남은 수량을 반환
    public int SetSlot(ItemData _data, int _amount)
    {
        data = _data;

        int remain = 0;
        if(_amount > data.MaxAmount)
        {
            amount = data.MaxAmount;
            remain = _amount - data.MaxAmount;
        }
        else
        {
            amount = _amount;
        }

        icon.sprite = data.Icon;
        icon.SetNativeSize();
        icon.enabled = true;

        UpdateAmountText();

        return remain;
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        data = null;
        amount = 0;

        icon.sprite = null;
        icon.enabled = false;

        UpdateAmountText();
        quickTxt.gameObject.SetActive(false);

        GameManager.Instance.UIManager.ItemButton.HideButtons();
    }

    // 해당 슬롯에 더 저장할 수 있는지 확인
    public bool CanSave(int id)
    {
        return !IsEmpty && (data.Id == id) && !isFull;
    }

    #region Amount
    // 아이템 양 추가 : 최대 수량을 벗어날 경우 최대까지 채우고 남은 수량을 반환
    public int AddAmount(int _amount)
    {
        int remain = 0;

        if(amount + _amount > data.MaxAmount)
        {
            remain = amount + _amount - data.MaxAmount;
            amount = data.MaxAmount;
        }
        else
        {
            amount += _amount;
        }

        UpdateAmountText();

        return remain;
    }

    // 아이템 양 감소
    public void DecreaseAmount(int _amount)
    {
        amount -= _amount;

        if(amount == 0)
        {
            ClearSlot();
        }
        else
        {
            UpdateAmountText();
        }
    }

    // 아이템 수량 반환
    public int GetAmount() => amount;

    // 수량 텍스트 업데이트
    private void UpdateAmountText()
    {
        if(amount > 1)
        {
            amountTxt.text = amount.ToString();
            amountTxt.gameObject.SetActive(true);
        }
        else
        {
            amountTxt.gameObject.SetActive(false);
        }
    }
    #endregion

    // 드랍 아이템 정보 반환
    public GameObject GetDropItem() => data.DropPrefab;

    // 슬롯 버튼이 클릭되었을 때 실행
    private void OnSlotButtonClick()
    {
        if (IsEmpty)
        {
            GameManager.Instance.UIManager.ItemButton.HideButtons();
            return;
        }

        GameManager.Instance.UIManager.ItemButton.DisplayItemButtons(data.Type, index);
    }
}
