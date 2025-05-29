using UnityEngine;

public class Resource : MonoBehaviour, IDamagable
{
    [Header("Resource")]
    [SerializeField] GameObject baseResource;
    [SerializeField] GameObject reamainResource;
    Collider baseCollider;
    [SerializeField] int[] equipmentId;
    [SerializeField] int maxAmount;
    int remain;
    bool isRemain => remain > 0;
    bool prevDayState;

    [Header("Drop Item")]
    [SerializeField] GameObject dropItem;
    [SerializeField] float innerRadius;
    [SerializeField] float range = 0.3f;
    [SerializeField] Color gizmoColor = new Color(1f, 0.5f, 0f, 1f); // 주황
    int segments = 64;

    private void Awake()
    {
        baseCollider = GetComponent<Collider>();

        Init();
    }

    private void Update()
    {
        // 아침이 되면 다시 재생성
        if(prevDayState == true && !GameManager.Instance.DayCycle.IsNight && !isRemain)
        {
            Init();
        }
        prevDayState = GameManager.Instance.DayCycle.IsNight;
    }

    // 리소스 초기화
    public void Init()
    {
        remain = maxAmount;

        baseResource.SetActive(true);
        baseCollider.enabled = true;

        reamainResource.SetActive(false);
    }

    // 아이템 떨어질 때의 랜덤 포지션 반환
    private Vector3 GetRandomPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float radius = Random.Range(innerRadius, innerRadius + range);
        Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0.5f, Mathf.Sin(angle) * radius);
        return transform.position + offset;
    }

    // 리소스가 데미지를 입으면 아이템을 드랍
    public void TakeDamage(int damage)
    {
        if (!isRemain || !IsCorrectTool()) return;

        remain--;
        Instantiate(dropItem, GetRandomPosition(), Quaternion.identity);

        if(!isRemain)
        {
            baseResource.SetActive(false);
            baseCollider.enabled = false;

            reamainResource.SetActive(true);
        }
    }

    // 특정 도구로 채취하고 있는 지 검사
    private bool IsCorrectTool()
    {
        if (equipmentId == null) return true;

        foreach(int id in equipmentId)
        {
            if (GameManager.Instance.Player.Equipment.CurID == id)
                return true;
        }

        return false;
    }

    #region OnEditor
    // 원 기즈모 그리기
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        DrawCircle(innerRadius);
        DrawCircle(innerRadius + range);
    }

    // 원 그리기
    private void DrawCircle(float radius)
    {
        Vector3 center = transform.position;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
    #endregion
}
