using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Interactable UI")]
    [SerializeField] GameObject interactableInfo;
    [SerializeField] Text interactableName;
    [SerializeField] Text interactableDesc;

    [Header("Inventory UI")]
    [SerializeField] GameObject inventoryUI;
    [SerializeField] ItemButton itemButton;
    public ItemButton ItemButton => itemButton;
    [SerializeField] SetQuickslotButton setQuickslotButton;
    public SetQuickslotButton SetQuickslotButton => setQuickslotButton;

    [Header("Architect UI")]
    [SerializeField] TabManager tabManager;

    public bool IsUIActive { get; private set; }

    // UI 매니저 초기화
    public void Init()
    {
        itemButton.Init();
        setQuickslotButton.Init();

        SetInteractableInfo(null);
        SetInventoryUI(false);
    }

    // 상호작용 가능한 오브젝트 정보 표시 / 숨기기
    public void SetInteractableInfo(IInteractable interactable)
    {
        if(interactable == null)
        {
            interactableInfo.SetActive(false);
            return;
        }

        interactableName.text = interactable.GetInteractName();
        interactableDesc.text = interactable.GetInteractDescription();

        interactableInfo.SetActive(true);
    }

    // 인벤토리 UI 띄우기 / 숨기기
    public void SetInventoryUI()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
        if(!inventoryUI.activeSelf) setQuickslotButton.HideButtons();

        ControlCursor(inventoryUI.activeSelf);
    }
    public void SetInventoryUI(bool show)
    {
        inventoryUI.SetActive(show);
        if(!show) setQuickslotButton.HideButtons();

        ControlCursor(show);
    }

    // 제작 UI 띄우기 / 숨기기
    public void SetArchitectUI()
    {
        tabManager.SetMenu();

        ControlCursor(tabManager.IsActive);
    }

    // UI가 켜지고 꺼짐에 따라 마우스 커서를 보이거나 안 보이게 설정
    private void ControlCursor(bool show)
    {
        IsUIActive = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
