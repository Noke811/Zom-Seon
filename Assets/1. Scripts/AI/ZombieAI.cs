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
    public float wanderRadius = 3f;         // 배회 시 이동할 수 있는 최대 반경
    public float detectRangeDay = 3f;        // 낮에 플레이어를 감지할 수 있는 거리
    public float fieldOfView = 60f;          // 낮에 플레이어를 감지할 수 있는 시야각
    public float attackRange = 1f;           // 공격 가능한 거리
    public float attackCooldown = 1f;        // 공격 간격 (초 단위)
    public int damage = 10;                  // 한 번의 공격으로 입히는 데미지량
    public float chaseKeepRange = 10f;       // 낮에 플레이어를 쫓다가 중단하는 거리

    [Header("참조")]
    public Transform player;                 // 추적 대상이 될 플레이어 Transform
    public LayerMask attackTargetMask;              // Fence 오브젝트를 감지하기 위한 레이어 마스크

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
    private bool isNight => GameManager.Instance.DayCycle.IsNight; // 현재 밤 여부 (GameManager 통해 확인)
    
    private ZombieStats stats;               // 좀비스텟
    
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();   // 경로 탐색 및 이동 제어
        animator = GetComponent<Animator>();    // 애니메이션 제어
        stats = GetComponent<ZombieStats>();    // 스탯 정보
    }

    private void Start()
    {
        // 플레이어가 설정되어 있지 않다면 태그로 찾아서 자동 할당
        if (player == null)
        {
            GameObject found = GameObject.FindWithTag("Player");
            if (found != null) player = found.transform;
        }

        // 첫 프레임에서 밤/낮 변화 상태를 초기화하고 감지를 강제로 수행
        wasNight = !isNight;               // 이전 상태를 현재와 반대로 설정해 Update에서 감지 로직 작동하게 유도
        DetectPlayerOrFence();             // 감지 시스템 최초 1회 작동
        OnResurrected();                   // 좀비 부활 초기화 설정
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
                TryAttackTargets();
                break;

            case ZombieState.AttackingFence:
                TryAttackTargets();      // Fence 공격 로직 실행
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
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 낮이면 일정 거리 넘어서면 추적 멈춤
        if (!isNight && distance > chaseKeepRange)
        {
            SetWandering();
            return;
        }

        // 가까우면 멈추고 바라보기만
        if (distance <= agent.stoppingDistance)
        {
            agent.isStopped = true;

            // 플레이어 바라보게 회전
            Vector3 dir = player.position - transform.position;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.forward = dir.normalized;

            return;
        }

        // 그 외에는 계속 추적
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }
    
    // 전방의 공격 가능 대상을 감지하고 공격 시도
    void TryAttackTargets()
    {
        // 마지막 공격 이후 쿨타임이 지났는지 확인
        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;

            // 좀비 전방 1.5 유닛 지점, 반경 1 내의 충돌체 탐색 (Layer 제한 없음)
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * attackRange, attackRange, attackTargetMask);
            foreach (var hit in hits)
            {
                // 감지된 오브젝트 데미지 적용
                if (hit.TryGetComponent(out IDamagable target))
                {
                    target.TakeDamage(damage);
                    Debug.Log($"공격 성공: {hit.name}에게 {damage} 피해");
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * attackRange, attackRange);
    }

    // 배회 상태에서의 이동 및 대기 처리
    void WanderUpdate()
    {
        // 목적지에 도착했고 아직 대기 중이 아니라면
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            isWaiting = true;                              // 대기 상태 진입
            wanderDelay = Random.Range(1f, 2f);            // 다음 이동까지의 대기 시간 랜덤 설정
            lastWanderTime = Time.time;                    // 현재 시각 기록
            agent.isStopped = true;                        // 이동 중단
            //Debug.Log("[WanderUpdate] 도착 + 대기 시작");
        }

        // 대기 상태 중이고, 대기 시간이 지났으면
        if (isWaiting && Time.time - lastWanderTime >= wanderDelay)
        {
            isWaiting = false;                             // 대기 상태 해제
            SetNewWanderDestination();                     // 다음 배회 목적지 설정
            //Debug.Log("[WanderUpdate] 대기 끝 → 다음 지점으로 이동");
        }
    }


    // 배회상태일 때의 목적지를 무작위로 설정하고 해당 위치로 이동 시작
    void SetNewWanderDestination()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius); // 현재 위치 기준으로 반경 내 무작위 위치 선택

        // NavMeshAgent가 비활성화된 경우 재활성화하고 기본 설정 보장
        if (!agent.enabled) agent.enabled = true;
        agent.updatePosition = true;       // 위치 자동 업데이트 허용
        agent.updateRotation = true;       // 회전 자동 업데이트 허용

        agent.SetDestination(newPos);      // 이동할 위치 설정
        agent.isStopped = false;           // 이동 상태 재개
    }

    // 좀비 상태를 배회로 설정하고, 즉시 다음 목적지를 지정하여 이동 시작
    void SetWandering()
    {
        state = ZombieState.Wandering;         // 상태를 배회로 설정
        isWaiting = false;                     // 대기 상태 초기화
        wanderDelay = Random.Range(1f, 2f);    // 다음 대기까지의 랜덤 지연 시간 설정
        lastWanderTime = Time.time;            // 현재 시간 저장

        SetNewWanderDestination();             // 즉시 새로운 배회 목적지 설정 및 이동 시작
    }

    // 주변에서 가장 가까운 Fence를 찾아 해당 위치로 이동, 공격 상태로 전환
    void FindNearestFence()
    {
        Collider[] hits = Physics.OverlapSphere
            (transform.position + transform.forward * 1.5f, 1f, attackTargetMask);
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
                if (Vector3.Distance(origin, navHit.position) > 0.5f)
                    return navHit.position; // 유효한 위치 반환
            }
        }

        // 10번 시도 후에도 실패하면 원래 위치 반환
        return origin;
    }
    
    // 좀비가 다시 활성화될 때 호출됨. 상태 및 이동 정보 초기화
    public void OnResurrected()
    {
        // 컴포넌트 재캐싱 (null일 경우만)
        if (stats == null) stats = GetComponent<ZombieStats>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        // NavMeshAgent 기본 설정
        agent.speed = stats.moveSpeed;
        agent.acceleration = 30f;
        agent.angularSpeed = 360f;
        agent.stoppingDistance = 0.8f;
        agent.autoBraking = true;

        // 상태 초기화 (배회 상태로 시작)
        wasNight = !isNight;
        state = ZombieState.Wandering;
        isWaiting = false;

        // 배회 관련 타이머 초기화 및 즉시 시작
        wanderDelay = Random.Range(1f, 2f);
        lastWanderTime = Time.time;
        SetNewWanderDestination();
    }

}