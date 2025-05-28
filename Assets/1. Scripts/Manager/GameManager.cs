using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UIManager UIManager { get; private set; }
    public Transform Player { get; private set; }
    [SerializeField] Inventory inventory;
    public Inventory Inventory => inventory;

    [Header("Time Control")]
    public bool IsNight = false;

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
        Player = GameObject.FindWithTag("Player")?.transform;
        inventory.Init();
    }

    private void Update()
    {
        // 테스트용: N 키를 눌러 밤/낮 전환
        if (Input.GetKeyDown(KeyCode.N))
        {
            IsNight = !IsNight;
            Debug.Log(IsNight ? "밤이 되었습니다" : "낮이 되었습니다");
        }
    }

    #region Initialization
    public void Init(UIManager uiManager)
    {
        UIManager = uiManager;
    }
    #endregion
}
