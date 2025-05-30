using UnityEngine;

public class InteractableDetector : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] LayerMask interactableLayer;

    IInteractable curInteractable;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(transform.position, transform.forward * distance, Color.red);

        if (Physics.Raycast(ray, out hit, distance, interactableLayer))
        {
            // 상호작용 가능한 오브젝트가 감지되었을 때
            IInteractable hitInteractable = hit.collider.GetComponent<IInteractable>();

            if (hitInteractable != curInteractable)
            {
                UpdateInteractable(hitInteractable);
            }
        }
        else
        {
            // 상호작용 가능한 오브젝트가 없을 때
            if (curInteractable != null)
            {
                UpdateInteractable(null);
            }
        }
    }

    // 상호작용 가능한 오브젝트 감지에 의한 업데이트
    private void UpdateInteractable(IInteractable _interactable)
    {
        curInteractable = _interactable;

        GameManager.Instance.UIManager.PlayingUI.SetInteractableInfo(curInteractable);
    }

    // 상호작용 키를 눌렀을 때 실행
    public void Interaction()
    {
        // 상호작용 가능한 오브젝트가 없으면 리턴
        if (curInteractable == null) return;

        curInteractable.OnInteract();
    }
}
