using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return instance;
        }
    }

    public UIManager UIManager { get; private set; }
    public Transform Player { get; private set; }

    [Header("Time Control")]
    public bool IsNight = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        Player = GameObject.FindWithTag("Player")?.transform;
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
