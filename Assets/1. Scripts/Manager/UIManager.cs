using UnityEngine;
using UnityEngine.UI;

public enum UIState
{
    Start,
    Playing,
    Over,
}

public enum PlayingUIState
{
    None,
    Inventory,
    Craft,
    Option,
}

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

    [Header("Conversation UI")]
    [SerializeField] GameObject conversationPanel;
    Text conversationText;

    public UIState State { get; private set; }
    private PlayingUIState playingUIState;
    public bool IsUIActive => playingUIState != PlayingUIState.None;

    // UI 매니저 초기화
    public void Init()
    {
        itemButton.Init();
        setQuickslotButton.Init();

        conversationText = conversationPanel.GetComponentInChildren<Text>();

        SetInteractableInfo(null);

        ChangePlayingUIState(PlayingUIState.None);
    }

    // 게임 중 UI 상태 변경
    public void ChangePlayingUIState(PlayingUIState state)
    {
        SetInventoryUI(state == PlayingUIState.Inventory ? true : false);
        SetArchitectUI(state == PlayingUIState.Craft ? true : false);
        ControlCursor(state != PlayingUIState.None ? true : false);

        playingUIState = state;
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

    // 대화창 띄우기
    public void DisplayConversationUI(string dialogue)
    {
        conversationText.text = dialogue;
        conversationPanel.SetActive(true);
    }

    // 인벤토리 UI 띄우기 / 숨기기
    private void SetInventoryUI(bool show)
    {
        inventoryUI.SetActive(show);
        if(!show) setQuickslotButton.HideButtons();
    }

    // 제작 UI 띄우기 / 숨기기
    private void SetArchitectUI(bool show)
    {
        if (tabManager.IsActive == show) return;
        tabManager.SetMenu();
    }

    // UI가 켜지고 꺼짐에 따라 마우스 커서를 보이거나 안 보이게 설정
    private void ControlCursor(bool show)
    {
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
