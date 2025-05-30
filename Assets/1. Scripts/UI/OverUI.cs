using UnityEngine;
using UnityEngine.UI;

public class OverUI : MonoBehaviour, IBaseUI
{
    [SerializeField] GameObject panel;
    [SerializeField] Button restartBtn;
    [SerializeField] Button goTitleBtn;
    [SerializeField] Button exitBtn;

    private void Awake()
    {
        restartBtn.onClick.AddListener(OnClickGameReStart);
        goTitleBtn.onClick.AddListener(() => GameManager.Instance.UIManager.ChangeUIState(UIState.Start));
        exitBtn.onClick.AddListener(OnClickGameExit);
    }

    // 버튼 클릭 시 게임 시작
    private void OnClickGameReStart()
    {
        GameManager.Instance.GameStart();
    }

    // 버튼 클릭 시 게임 종료
    private void OnClickGameExit()
    {
        GameManager.Instance.GameExit();
    }
    
    // UI 상태 변경
    public void SetUIState(UIState state)
    {
        bool isStart = state == UIState.Over;
        panel.SetActive(isStart);
    }
}
