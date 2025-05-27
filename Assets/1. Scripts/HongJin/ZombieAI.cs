using UnityEngine;
using UnityEngine.AI;

public enum ZombieState
{
    Wandering,
    Chasing,
    AttackingFence
}

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("설정")]
    public float wanderRadius = 10f;         // 배회 시 이동할 수 있는 최대 반경
    public float detectRangeDay = 5f;        // 낮에 플레이어를 감지할 수 있는 거리
    public float fieldOfView = 60f;          // 낮에 플레이어를 감지할 수 있는 시야각
    public float attackRange = 2f;           // 공격 가능한 거리
    public float attackCooldown = 1f;        // 공격 간격 (초 단위)
    public int damage = 10;                  // 한 번의 공격으로 입히는 데미지량
    public float chaseKeepRange = 10f;       // 낮에 플레이어를 쫓다가 중단하는 거리

    [Header("참조")]
    public Transform player;                 // 추적 대상이 될 플레이어 Transform
    public LayerMask fenceMask;              // Fence 오브젝트를 감지하기 위한 레이어 마스크

    // 컴포넌트 및 상태 캐시
    private NavMeshAgent agent;              // 좀비의 경로 이동 처리용 NavMeshAgent
    private Animator animator;               // 애니메이션 제어용 Animator

    // 배회 관련 상태 제어 변수
    private float lastWanderTime;            // 마지막 배회 시도 시각
    private float wanderDelay;               // 다음 배회까지 대기 시간
    private bool isWaiting = false;          // 배회 중 정지 상태 여부

    // 공격 관련 타이머
    private float lastAttackTime;            // 마지막 공격 시각

    // 좀비의 현재 행동 상태
    private ZombieState state;               // 현재 좀비의 상태 (배회, 추적, 공격 등)

    // 밤/낮 상태 기록
    private bool wasNight = false;           // 이전 프레임이 밤이었는지 여부
    private bool isNight => GameManager.Instance.IsNight; // 현재 밤 여부 (GameManager 통해 확인)
    
    
    //컴포넌트 설정, 이동 속성 설정, 상태 초기화, 플레이어 찾기
    private void Start()
    {
        // 좀비 이동 및 애니메이션 제어용 컴포넌트 참조
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    
        // 이동 성능 설정 (즉각 반응하도록 튜닝)
        agent.acceleration = 30f;        // 빠르게 출발
        agent.angularSpeed = 360f;       // 빠르게 회전
        agent.stoppingDistance = 0.1f;   // 목적지에 가까워지면 정확히 멈춤
        agent.autoBraking = true;        // 멈출 때 천천히 제동하지 않고 즉시 정지

        // 시작 상태를 '배회'로 설정
        state = ZombieState.Wandering;

        // 플레이어 참조가 비어 있을 경우 자동 탐색
        if (player == null)
        {
            GameObject target = GameObject.FindWithTag("Player");
            if (target != null)
                player = target.transform;
        }

        // 첫 배회 목적지 설정 (랜덤 이동 시작)
        ScheduleNextWander();
    }

    //상태에 따라 행동 수행 + 애니메이션 + 감지 처리
    private void Update()
    {

        // 이동 여부 → 애니메이션 제어
        bool isMoving = agent.velocity.magnitude > 0.25f;
        animator.SetBool("isRun", isMoving);

        // 밤 → 낮으로 전환된 경우 배회 상태로 되돌림
        if (!isNight && wasNight)
            SetWandering();
        // 낮 → 밤으로 전환된 경우 감지 재시작
        else if (isNight && !wasNight)
            DetectPlayerOrFence();
        

        // 현재 시점의 밤/낮 상태 저장
        wasNight = isNight;

        // 현재 상태에 따라 각각의 행동 실행
        switch (state)
        {
            case ZombieState.Wandering:
                WanderUpdate();            // 배회 중이면 이동 여부 및 대기 시간 처리
                DetectPlayerOrFence();    // 플레이어 또는 Fence 감지
                break;

            case ZombieState.Chasing:
                ChaseUpdate();            // 플레이어 추적 및 사거리 확인
                DetectPlayerOrFence();    // 감지 갱신 (추적 중에도 재판단)
                break;

            case ZombieState.AttackingFence:
                FenceAttackUpdate();      // Fence 공격 로직 실행
                DetectPlayerOrFence();    // Fence 공격 중에도 플레이어 추적 가능해지면 전환
                break;
        }
    }

    // 현재 상황에 따라 플레이어 또는 Fence를 감지하여 상태 전환을 수행
    void DetectPlayerOrFence()
    {
        // 플레이어가 없으면 감지 중단
        if (player == null) return;

        if (isNight)
        {
            // 밤: 시야각/거리 무시하고, 플레이어에게 도달 가능한 경로가 있는지만 확인
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(player.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                // 경로가 있으면 플레이어 추적 시작
                state = ZombieState.Chasing;
            }
            else
            {
                // 경로가 없으면 (즉, Fence 등으로 막혔으면) Fence를 감지하여 공격 상태로 전환
                FindNearestFence();
            }
        }
        else
        {
            // 낮: 거리 + 시야각 기준으로 플레이어를 감지
            float distance = Vector3.Distance(player.position, transform.position);
            Vector3 dir = player.position - transform.position;
            float angle = Vector3.Angle(transform.forward, dir);

            // 감지 범위와 시야각 조건을 모두 만족하면 추적 시작
            if (distance <= detectRangeDay && angle <= fieldOfView * 0.5f)
                state = ZombieState.Chasing;
        }
    }

    // 플레이어 추적 중일 때 실행되는 로직
    void ChaseUpdate()
    {
        // 플레이어가 없으면 아무것도 하지 않음
        if (player == null) return;

        // 플레이어와의 현재 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // 낮일 때, 플레이어가 추적 유지 거리보다 멀어지면 배회 상태로 전환
        if (!isNight && distance > chaseKeepRange)
        {
            SetWandering();
            return;
        }

        // NavMeshAgent를 활성화하고, 플레이어 위치를 목적지로 설정
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // 플레이어가 공격 사거리 안에 들어온 경우
        if (distance <= attackRange)
        {
            Debug.Log("좀비가 플레이어 공격!");
            // TODO: 공격 애니메이션, 데미지 처리 등 추가 예정
        }
    }
    
    // Fence를 공격하는 상태에서 실행되는 로직
    void FenceAttackUpdate()
    {
        // 마지막 공격 이후 쿨타임이 지나지 않았다면 공격하지 않음
        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;

            // 좀비 전방 1.5, 반경 1 내에 있는 Fence를 감지
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 1.5f, 1f, fenceMask);
        
            foreach (var hit in hits)
            {
                // 감지된 오브젝트에 SimpleHealth 컴포넌트가 있다면 데미지 적용
                if (hit.TryGetComponent(out SimpleHealth health))
                {
                    health.TakeDamage(damage);
                    Debug.Log("Fence 공격!");
                    // TODO: 공격 이펙트, 애니메이션 연동 가능
                }
            }
        }
    }

    // 배회(Wandering) 상태에서의 이동 및 대기 처리
    void WanderUpdate()
    {
        // 현재 목적지까지 도착했고 대기 상태가 아닐 경우 → 대기 시작
        if (!agent.pathPending && agent.remainingDistance <= 0.5f && !isWaiting)
        {
            isWaiting = true;                                 // 대기 상태 진입
            wanderDelay = Random.Range(1f, 2f);               // 1~2초 사이 대기 시간 설정
            lastWanderTime = Time.time;                       // 대기 시작 시각 저장
            agent.isStopped = true;                           // 이동 중단
        }

        // 대기 시간이 모두 지난 경우 → 다음 배회 지점 설정
        if (isWaiting && Time.time - lastWanderTime >= wanderDelay)
        {
            isWaiting = false;                                // 대기 종료
            SetNewWanderDestination();                        // 새로운 랜덤 목적지 설정
        }
    }

    // 배회 상태일 때 다음 이동까지의 대기 시간과 목적지를 설정함
    void ScheduleNextWander()
    {
        wanderDelay = Random.Range(1f, 2f);        // 대기 시간 1~2초 랜덤 설정
        lastWanderTime = Time.time;                // 현재 시간을 기준으로 대기 시작
        SetNewWanderDestination();                 // 다음 이동 지점 설정
    }

    // NavMesh 위에서 배회 목적지를 무작위로 지정하여 이동 시작
    void SetNewWanderDestination()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius); // 반경 내 랜덤 위치
        agent.SetDestination(newPos);          // NavMeshAgent에 목적지 설정
        agent.isStopped = false;               // 이동 재개
    }

    // 좀비 상태를 Wandering으로 전환하고 초기 배회 시작
    void SetWandering()
    {
        state = ZombieState.Wandering;         // 상태 전환
        ScheduleNextWander();                  // 다음 배회 설정 (이동 지점 + 대기 시간)
    }

    // 주변에서 가장 가까운 Fence를 찾아 해당 위치로 이동, 공격 상태로 전환
    void FindNearestFence()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 50f, fenceMask);
        Debug.Log("Fence 감지 시도, 감지된 개수: " + hits.Length);

        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Fence"))
            {
                float dist = Vector3.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.transform;
                }
            }
        }

        if (nearest != null)
        {
            // 가장 가까운 Fence를 찾았으면 해당 위치로 이동하고 공격 상태로 전환
            Debug.Log("가장 가까운 Fence 발견, 추적 시작");
            agent.SetDestination(nearest.position);
            state = ZombieState.AttackingFence;
        }
        else
        {
            // Fence를 못 찾았으면 배회 상태로 복귀
            SetWandering();
        }
    }

    // 주어진 위치(origin) 기준으로 NavMesh 상의 랜덤한 유효 위치를 반환하는 함수
    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        for (int i = 0; i < 10; i++) // 최대 10회 시도
        {
            // 반지름 dist 안에서 랜덤한 방향의 위치 계산
            Vector3 randDir = Random.insideUnitSphere * dist + origin;

            // 해당 위치가 NavMesh 위에 존재하는지 확인
            if (NavMesh.SamplePosition(randDir, out NavMeshHit navHit, dist, NavMesh.AllAreas))
            {
                // origin에서 너무 가까운 지점은 제외 (2.5f 이상 떨어져야 함)
                if (Vector3.Distance(origin, navHit.position) > 2.5f)
                    return navHit.position; // 유효한 위치 반환
            }
        }

        // 10번 시도 후에도 실패하면 원래 위치 반환
        return origin;
    }
}