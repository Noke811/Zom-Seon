using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsPlaying { get; private set; }

    [SerializeField] LayerMask excludeLayer;

    [Header("Objects")]
    [SerializeField] Player player;
    public Player Player => player;
    [SerializeField] UIManager uiManager;
    public UIManager UIManager => uiManager;
    [SerializeField] Inventory inventory;
    public Inventory Inventory => inventory;
    [SerializeField] DayAndNight dayCycle;
    public DayAndNight DayCycle => dayCycle;
    [SerializeField] ZombieSpawner spawner;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Camera.main.cullingMask = ~excludeLayer;

        IsPlaying = false;
        uiManager.ChangeUIState(UIState.Start);
    }

    // 게임 시작
    public void GameStart()
    {
        uiManager.ChangeUIState(UIState.Playing);
        inventory.Init();
        dayCycle.Init();
        Player.Condition.OnRevive();

        IsPlaying = true;
    }

    // 플레이 중에 게임을 멈춤 : NPC 대화 등에 사용
    public void GamePause(bool isPause)
    {
        IsPlaying = !isPause;
    }

    // 게임 오버
    public void GameOver()
    {
        IsPlaying = false;

        spawner.DieAllZombies();
        uiManager.ChangeUIState(UIState.Over);
    }

    // 게임 종료
    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
