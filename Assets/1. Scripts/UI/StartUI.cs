using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour, IBaseUI
{
    [SerializeField] GameObject panel;
    [SerializeField] Button startBtn;
    [SerializeField] Button exitBtn;

    private void Awake()
    {
        startBtn.onClick.AddListener(OnClickGameStart);
        exitBtn.onClick.AddListener(OnClickGameExit);
    }

    // 버튼 클릭 시 게임 시작
    private void OnClickGameStart()
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
        bool isStart = state == UIState.Start;
        panel.SetActive(isStart);
    }
}
