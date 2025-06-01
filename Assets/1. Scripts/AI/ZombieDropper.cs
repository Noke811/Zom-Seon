using UnityEngine;

[System.Serializable]
public class DropItemData
{
    public GameObject itemPrefab;                        // 드롭할 아이템 프리팹
    [Range(0f, 1f)] public float dropProbability = 1f;   // 해당 아이템의 개별 드롭 확률
}

public class ZombieDropper : MonoBehaviour
{
    [Header("드롭 아이템 (개별 확률)")]
    public DropItemData[] dropItems;                     // 드롭 가능한 아이템 목록

    // 아이템 드롭
    public void DropItems()
    {
        Debug.Log($"[DropItems] 호출됨 - 대상 수: {dropItems.Length}");

        foreach (var data in dropItems)
        {
            if (data.itemPrefab == null)
            {
                Debug.LogWarning("[DropItems] itemPrefab이 비어 있음");
                continue;
            }

            float roll = Random.value;
            Debug.Log($"[DropItems] {data.itemPrefab.name} 드롭 확률: {data.dropProbability}, 랜덤값: {roll}");
            
            if (roll <= data.dropProbability)
            {
                // 충돌방지
                Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));
                Vector3 dropPos = transform.position + offset + Vector3.up * 0.3f;

                // 아이템 생성
                GameObject dropped = Instantiate(data.itemPrefab, dropPos, Quaternion.identity);
            }
        }
    }
}