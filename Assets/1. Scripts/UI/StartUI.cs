using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour, IBaseUI
{
    [SerializeField] GameObject panel;       // 시작 화면 전체 패널
    [SerializeField] Button startBtn;        // 게임 시작 버튼
    [SerializeField] Button exitBtn;         // 게임 종료 버튼

    [SerializeField] Button optionBtn;          // 옵션 버튼 (사운드 설정 열기용)
    [SerializeField] GameObject soundVolume;    // 사운드 설정 UI 패널
    [SerializeField] Button closeSoundBtn;      // 사운드 설정 닫기 버튼

    private void Awake()
    {
        startBtn.onClick.AddListener(OnClickGameStart);
        exitBtn.onClick.AddListener(OnClickGameExit);
        optionBtn.onClick.AddListener(OnClickOption);
        closeSoundBtn.onClick.AddListener(OnClickCloseSoundUI);
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

    // 버튼 클릭 시 사운드 설정 UI 열기
    private void OnClickOption()
    {
        soundVolume.SetActive(true);
    }

    // 닫기 버튼 클릭 시 사운드 설정 UI 닫기
    private void OnClickCloseSoundUI()
    {
        soundVolume.SetActive(false);
    }

    // UI 상태 변경
    public void SetUIState(UIState state)
    {
        bool isStart = state == UIState.Start;
        panel.SetActive(isStart);
    }
}
