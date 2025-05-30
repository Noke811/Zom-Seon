using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour
{
    [Header("좀비 프리팹")]
    public GameObject zombiePrefab;        // 일반 좀비 프리팹
    public GameObject zombie2Prefab;        // 강화 좀비 프리팹

    [Header("스폰 영역 (BoxCollider 포함)")]
    public Transform[] spawnZones;         // 스폰 구역으로 사용할 오브젝트들 (BoxCollider 필요)

    [Header("스폰 설정")]
    public float spawnInterval = 5f;       // 스폰 주기 (초 단위)
    public int spawnCountPerInterval = 3;  // 한번에 스폰할 좀비 수
    public int maxZombieCount = 20;        // 최대 활성 좀비 수 제한
    public float eliteZombieChance = 0.2f; // 강화확률

    [Header("풀 크기")]
    public int poolSize = 10;              // 각 좀비 종류당 초기 풀 사이즈

    // 오브젝트 풀: 비활성화된 좀비 저장용
    private Queue<GameObject> zombiePool = new Queue<GameObject>();
    private Queue<GameObject> zombie2Pool = new Queue<GameObject>();

    // 현재 활성화된 좀비 추적용 리스트
    private List<GameObject> activeZombies = new List<GameObject>();

    private float lastSpawnTime;
    
    void Start()
    {
        // 풀 초기화
        InitPool(zombiePrefab, zombiePool);
        InitPool(zombie2Prefab, zombie2Pool);

        lastSpawnTime = Time.time;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;

        // sapwnInterval마다 좀비 생성
        if(Time.time - lastSpawnTime > spawnInterval)
        {
            lastSpawnTime = Time.time;
            SpawnZombies();
        }
    }

    // 오브젝트 풀에 미리 좀비 생성하여 넣기
    void InitPool(GameObject prefab, Queue<GameObject> pool)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    // 주기적으로 호출되는 좀비 생성 함수
    void SpawnZombies()
{
    // 현재 살아 있는 좀비 수 확인
    int aliveCount = activeZombies.Count(z => z.activeInHierarchy);

    // 최대치 도달 시 스폰 중단
    if (aliveCount >= maxZombieCount) return;

    // 설정된 수만큼 반복
    for (int i = 0; i < spawnCountPerInterval; i++)
    {
        // 반복 도중 최대치 도달하면 중단
        if (activeZombies.Count(z => z.activeInHierarchy) >= maxZombieCount)
            break;

        // 스폰 구역 하나 선택 후 랜덤 위치 가져오기
        Transform zone = spawnZones[Random.Range(0, spawnZones.Length)];
        Vector3 spawnPos = GetRandomPositionInZone(zone);

        // NavMesh 위 위치로 보정 (네비 메시 경계 문제 방지)
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2.0f, NavMesh.AllAreas))
        {
            spawnPos = hit.position; // 가장 가까운 네비 메시 지점으로 보정
        }
        else
        {
            Debug.LogWarning($"[스폰 취소] NavMesh 위가 아닌 위치입니다: {spawnPos}");
            continue; // 스폰 건너뜀
        }

        // 강화 좀비 여부 확률 계산
        bool isElite = Random.value < eliteZombieChance;

        // 풀에서 꺼내기 (없으면 새로 생성)
        GameObject zombieToSpawn = isElite
            ? GetFromPool(zombie2Pool, zombie2Prefab)
            : GetFromPool(zombiePool, zombiePrefab);

        // 먼저 비활성화해서 NavMeshAgent 초기화 차단
        zombieToSpawn.SetActive(false);

        // 위치 지정
        zombieToSpawn.transform.position = spawnPos;

        // NavMeshAgent 강제로 재설정 (Warp 방식 사용)
        NavMeshAgent agent = zombieToSpawn.GetComponent<NavMeshAgent>();
        agent.enabled = false;
        agent.Warp(spawnPos); // NavMesh 위로 강제 배치
        agent.enabled = true;

        // 활성화
        zombieToSpawn.SetActive(true);

        // 스탯 설정
        ZombieStats stats = zombieToSpawn.GetComponent<ZombieStats>();
        stats.spawner = this;
        stats.isElite = isElite;
        stats.ResetHealth();

        // 좀비 초기화 메서드 호출
        zombieToSpawn.GetComponent<ZombieAI>().OnResurrected();

        // 리스트에 등록 (중복 방지)
        if (!activeZombies.Contains(zombieToSpawn))
            activeZombies.Add(zombieToSpawn);
    }
}

    // 특정 스폰 구역 안에서 랜덤한 위치 반환
    Vector3 GetRandomPositionInZone(Transform zone)
    {
        BoxCollider box = zone.GetComponent<BoxCollider>();
        Vector3 center = box.bounds.center;
        Vector3 extents = box.bounds.extents;

        float x = Random.Range(center.x - extents.x, center.x + extents.x);
        float z = Random.Range(center.z - extents.z, center.z + extents.z);
        float y = center.y;

        return new Vector3(x, y, z);
    }

    // 오브젝트 풀에서 좀비 꺼내기 (없으면 생성)
    GameObject GetFromPool(Queue<GameObject> pool, GameObject prefab)
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        else
            return Instantiate(prefab);
    }

    // 좀비 사망 또는 비활성화 시 호출 → 풀로 복귀
    public void ReturnToPool(GameObject zombie, bool isElite)
    {
        zombie.SetActive(false);

        if (isElite)
            zombie2Pool.Enqueue(zombie);  // 강화 좀비는 강화용 풀로
        else
            zombiePool.Enqueue(zombie);   // 일반 좀비는 일반 풀로

        if (activeZombies.Contains(zombie))
            activeZombies.Remove(zombie); // 활성 리스트에서 제거
    }

    // Scene 뷰에서 스폰 구역 시각화 (녹색 박스)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (spawnZones != null)
        {
            foreach (Transform zone in spawnZones)
            {
                BoxCollider box = zone.GetComponent<BoxCollider>();
                if (box != null)
                {
                    Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
                }
            }
        }
    }
}
