using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField] Transform pivot;
    Dictionary<int, GameObject> equipObject = new Dictionary<int, GameObject>();
    public int CurID { get; private set; }
    public bool IsEquip => CurID != -1;
    [SerializeField] Animator animator;
    
    private void Awake()
    {
        for(int i = 0; i < pivot.childCount; i++)
        {
            GameObject equip = pivot.GetChild(i).gameObject;
            equipObject[200 + i] = equip;
            equip.SetActive(false);
        }

        CurID = -1;
    }

    // 장비 착용
    public void Equip(int id)
    {
        if (equipObject.ContainsKey(id))
        {
            if (IsEquip)
            {
                equipObject[CurID].SetActive(false);
            }

            equipObject[id].SetActive(true);
            CurID = id;
        }
    }

    // 장비 해제
    public void Unequip(int id)
    {
        if (!IsEquip || CurID != id) return;

        equipObject[CurID].SetActive(false);
        CurID = -1;
    }

    // 공격
    public void Attack()
    {
        if (IsEquip)
        {
            animator.SetTrigger("Attack");
            GameManager.Instance.Player.Detector.AttackDamagables(GameManager.Instance.Player.Stat.FinalAtk);
        }
    }
}
