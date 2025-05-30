using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPC_Interact : MonoBehaviour, IInteractable
{
    public NPC_AI ai { get; private set; }
    public PlayerController player;

    [TextArea]
    public string dialogue = "안녕하세요! 무엇을 도와드릴까요?";
    private bool isTalking = false;
    public float textSpeed = 0.05f;

    private void Awake()
    {
        ai = GetComponent<NPC_AI>();

        // 플레이어 자동 찾기
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.GetComponent<PlayerController>();
        }
    }

    public string GetInteractName() => "NPC";
    public string GetInteractDescription() => "F - 대화하기";

    public void OnInteract()
    {
        if (!isTalking)
        {
            ai.StopMoving();
            FacePlayer(Camera.main.transform);

            Time.timeScale = 0f;

            // 대화 UI 켜기 (UIManager에게 책임 넘김)
            GameManager.Instance.UIManager.SetDialogueUI(true, dialogue);

            StartCoroutine(TypeDialogue(dialogue));
            isTalking = true;
        }
        else
        {
            // 대화 UI 끄기
            GameManager.Instance.UIManager.SetDialogueUI(false);
            Time.timeScale = 1f;
            isTalking = false;
        }
    }

    public void FacePlayer(Transform playerTransform)
    {
        Vector3 dir = playerTransform.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.forward = dir.normalized;
    }

    private IEnumerator TypeDialogue(string line)
    {
        Text dialogueText = GameManager.Instance.UIManager.GetDialogueText();
        if (dialogueText == null)
            yield break;

        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }
}