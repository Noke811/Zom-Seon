using UnityEngine;
using UnityEngine.UI;

public enum PlayingUIState
{
    None,
    Inventory,
    Craft,
    Conversation,
}

public class PlayingUI : MonoBehaviour, IBaseUI
{
    [SerializeField] GameObject panel;
    bool isInit = false;
    public PlayingUIState PlayingUIState { get; private set; }
    public bool IsUIActive => PlayingUIState != PlayingUIState.None;

    [Header("Interactable UI")]
    [SerializeField] GameObject interactableInfo;
    [SerializeField] Text interactableName;
    [SerializeField] Text interactableDesc;

    [Header("Player Info")]
    [SerializeField] UICondition uiCondition;
    public UICondition UICondition => uiCondition;

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
    [SerializeField] Button closeButton;
    private NPC_Interact currentNPCINteractor;


    // 플레잉 UI 초기화
    private void Init()
    {
        itemButton.Init();
        setQuickslotButton.Init();

        conversationText = conversationPanel.GetComponentInChildren<Text>();

        SetInteractableInfo(null);

        isInit = false;
    }

    // 게임 중 UI 상태 변경
    public void ChangePlayingUIState(PlayingUIState state)
    {
        SetInventoryUI(state == PlayingUIState.Inventory);
        SetArchitectUI(state == PlayingUIState.Craft);
        SetConversationUI(state == PlayingUIState.Conversation);
        GameManager.Instance.UIManager.ControlCursor(state != PlayingUIState.None);

        PlayingUIState = state;
    }

    // 상호작용 가능한 오브젝트 정보 표시 / 숨기기
    public void SetInteractableInfo(IInteractable interactable)
    {
        if (interactable == null)
        {
            interactableInfo.SetActive(false);
            return;
        }

        interactableName.text = interactable.GetInteractName();
        interactableDesc.text = interactable.GetInteractDescription();

        interactableInfo.SetActive(true);
    }

    // 대화창 띄우기
    private void SetConversationUI(bool show)
    {
        conversationText.text = "";
        conversationPanel.SetActive(show);
    }

    // 대화 텍스트 오브젝트 반환
    public Text GetConversationText()
    {
        return conversationText;
    }
    

    // 인벤토리 UI 띄우기 / 숨기기
    private void SetInventoryUI(bool show)
    {
        inventoryUI.SetActive(show);
        if (!show) setQuickslotButton.HideButtons();
    }

    // 제작 UI 띄우기 / 숨기기
    private void SetArchitectUI(bool show)
    {
        if (tabManager.IsActive == show) return;
        tabManager.SetMenu();
    }

    // UI 상태 변경
    public void SetUIState(UIState state)
    {
        bool isPlaying = state == UIState.Playing;

        panel.SetActive(isPlaying);

        if (isPlaying && !isInit)
        {
            Init();
        }
    }

    public void SetUpNPCInteract(NPC_Interact npc, string dialogue)
    {
        this.currentNPCINteractor = npc;
        this.conversationText.text = dialogue;
        if (closeButton != null && this.currentNPCINteractor != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickCloseButton);
        }
    }
    private void OnClickCloseButton()
    {
        currentNPCINteractor.EndConversation();
    }
}
