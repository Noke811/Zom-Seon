using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamagableDetector : MonoBehaviour
{
    Dictionary<IDamagable, bool> damagables = new Dictionary<IDamagable, bool>();

    private void FixedUpdate()
    {
        // 모든 damagables 상태 false로 초기화
        var keys = damagables.Keys.ToList();
        foreach (var key in keys)
            damagables[key] = false;
    }

    public void AttackDamagables(int damage)
    {
        // false인 damagables 딕셔너리에서 삭제
        var remove = damagables.Where(dict => !dict.Value).Select(dict => dict.Key).ToList();
        foreach(var key in remove)
        {
            damagables.Remove(key);
        }

        // damagables 딕셔너리에 포함된, 즉 감지되 오브젝트에 대해서 데미지 줌
        foreach (IDamagable damagable in damagables.Keys)
        {
            damagable.TakeDamage(damage);
            GameManager.Instance.Player.Condition.AttackStamina();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger Stay with: " + other.name);

        if (other.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagables[damagable] = true;
        }
    }
}
