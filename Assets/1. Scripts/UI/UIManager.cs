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

    private void Awake()
    {
        GameManager.Instance.Init(this);
    }

    private void Start()
    {
        SetInteractableInfo(null);
        inventoryUI.SetActive(false);
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
    }
}
