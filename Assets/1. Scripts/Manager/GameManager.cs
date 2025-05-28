using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] Player player;
    public Player Player => player;
    [SerializeField] UIManager uiManager;
    public UIManager UIManager => uiManager;
    [SerializeField] Inventory inventory;
    public Inventory Inventory => inventory;
    public DayAndNight DayCycle { get; private set; }

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

        uiManager.Init();
        inventory.Init();

        DayCycle = GetComponentInChildren<DayAndNight>();
    }
}
