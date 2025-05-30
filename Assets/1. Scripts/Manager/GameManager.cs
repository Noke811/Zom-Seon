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

    public void GameStart()
    {
        IsPlaying = true;
        uiManager.ChangeUIState(UIState.Playing);
        inventory.Init();
    }

    public void GameOver()
    {
        IsPlaying = false;
        uiManager.ChangeUIState(UIState.Over);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
