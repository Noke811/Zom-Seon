using UnityEngine;

public class CharacterStat : MonoBehaviour
{
    [Header("Stat")]
    public int baseAtk;
    public int baseDef;
    private int equipAtkBonus;
    private int equipDefBonus;
    public int FinalAtk
    {
        get
        {
            int totalAtk = baseAtk + equipAtkBonus;
            return totalAtk;
        }
    }
    public int FinalDef
    {
        get
        {
            int totalDef = baseDef + equipDefBonus;
            return totalDef;
        }
    }
    
    public void Add(BuffType buff, int value)
    {
        // 장비된 아이템의 버프 타입이 Attack이면
        if (buff == BuffType.Attack)
            equipAtkBonus = value;
        // equipAtkBonus 증가
    }

    public void Subtract(BuffType buff)
    {
        if (buff == BuffType.Attack)
            equipAtkBonus = 0;
    }
}