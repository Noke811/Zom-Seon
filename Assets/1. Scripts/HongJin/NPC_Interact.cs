using UnityEngine;

public class NPC_Interact : MonoBehaviour, IInteractable
{
    public NPC_AI ai { get; private set; }

    [TextArea]
    public string dialogue = "안녕하세요! 무엇을 도와드릴까요?";

    private void Awake()
    {
        ai = GetComponent<NPC_AI>();
    }

    public string GetInteractName() => "NPC";

    public string GetInteractDescription() => "F - 대화하기";

    public void OnInteract()
    {
        // PlayerController에서 처리
    }

    public void FacePlayer(Transform player)
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.forward = dir.normalized;
    }
    
    // 플레이어 컨트롤
    /*[Header("대화")]
    private GameObject dialoguePanel;
    private Text dialogueText;
    
    private InteractableDetector detector;
    
    private void Start()
    {
        detector = GetComponent<InteractableDetector>();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        IInteractable target = detector.GetCurrentTarget();

        if (target is NPC_Interact npc)
        {
            npc.ai.StopMoving();
            npc.FacePlayer(transform);

            dialogueText.text = npc.dialogue;
            dialoguePanel.SetActive(true);
        }
        else
        {
            target?.OnInteract();
        }
    }*/
    
}