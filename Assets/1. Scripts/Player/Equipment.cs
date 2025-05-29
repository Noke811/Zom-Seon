using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField] Transform pivot;
    Dictionary<int, GameObject> equipObject = new Dictionary<int, GameObject>();
    GameObject curEquip;

    public bool IsEquip => curEquip != null;

    private void Awake()
    {
        for(int i = 0; i < pivot.childCount; i++)
        {
            GameObject equip = pivot.GetChild(i).gameObject;
            equipObject[200 + i] = equip;
            equip.SetActive(false);
        }

        curEquip = null;
    }

    public void Equip(int id)
    {
        if (equipObject.ContainsKey(id))
        {
            if (curEquip != null)
            {
                curEquip.SetActive(false);
            }

            equipObject[id].SetActive(true);
            curEquip = equipObject[id];
        }
    }

    public void Unequip(int id)
    {
        if (equipObject.ContainsKey(id))
        {
            if (curEquip == equipObject[id])
            {
                curEquip.SetActive(false);
                curEquip = null;
            }
        }
    }

    public void Attack()
    {
        Debug.Log("공격!");   
    }
}
