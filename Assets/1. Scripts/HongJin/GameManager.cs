using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform Player { get; private set; }
    private void Start()
    {
        Player = GameObject.FindWithTag("Player")?.transform;
    }

    [Header("Time Control")]
    public bool IsNight = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
}
