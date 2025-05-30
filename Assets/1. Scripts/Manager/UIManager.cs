using UnityEngine;

public interface IBaseUI
{
    public void SetUIState(UIState state);
}

public enum UIState
{
    Start,
    Playing,
    Over,
}

public class UIManager : MonoBehaviour
{
    [SerializeField] StartUI startUI;
    [SerializeField] PlayingUI playingUI;
    public PlayingUI PlayingUI => playingUI;
    [SerializeField] OverUI overUI;

    // UI 상태 변경
    public void ChangeUIState(UIState state)
    {
        startUI.SetUIState(state);
        playingUI.SetUIState(state);
        overUI.SetUIState(state);

        ControlCursor(state != UIState.Playing);
    }

    // UI가 켜지고 꺼짐에 따라 마우스 커서를 보이거나 안 보이게 설정
    public void ControlCursor(bool show)
    {
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
    
    public void SetDialogueUI(bool show, string dialogue = "")
    {
        dialoguePanel.SetActive(show);

        if (show && !string.IsNullOrEmpty(dialogue))
            dialogueText.text = "";

        ControlCursor(show); 
    }
    
    public Text GetDialogueText()
    {
        return dialogueText;
    }
}
