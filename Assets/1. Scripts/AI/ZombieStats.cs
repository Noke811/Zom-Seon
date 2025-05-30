using UnityEngine;

// 좀비의 능력치와 체력 관리, 데미지 처리 등을 담당
public class ZombieStats : MonoBehaviour, IDamagable
{
    // 좀비의 최대 체력
    public int maxHealth = 100;

    // 좀비의 이동 속도
    public float moveSpeed = 3.5f;

    // 현재 체력
    private int currentHealth;

    // 스폰 시 설정되는 강화 좀비 여부
    [HideInInspector] public bool isElite;

    // 나를 생성한 ZombieSpawner를 저장
    [HideInInspector] public ZombieSpawner spawner;

    // 게임 오브젝트가 생성될 때 최대 체력으로 초기화
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // 외부에서 데미지를 입힐 때 호출
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"[ZombieStats] 데미지 {damage} 입음 → 남은 체력: {currentHealth}");

        // 체력이 0 이하가 되면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 사망 처리
    private void Die()
    {
        Debug.Log("[ZombieStats] 좀비 사망 처리 실행");

        // 스포너가 연결되어 있으면 풀로 반환, 없으면 비활성화
        if (spawner != null)
            spawner.ReturnToPool(gameObject, isElite);  // 풀에 다시 넣음
        else
            gameObject.SetActive(false);
    }

    // 좀비가 다시 스폰될 때 체력을 초기화
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}