using UnityEngine;
using UnityEngine.UI;

public class NPC_Interact : MonoBehaviour, IInteractable
{
    public NPC_AI ai { get; private set; }
    public Text dialogueText;
    public GameObject dialoguePanel;

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
        ai.StopMoving();
        //FacePlayer();

        dialogueText.text = dialogue;
        dialoguePanel.SetActive(true);
    }

    public void FacePlayer(Transform player)
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.forward = dir.normalized;
    }
}