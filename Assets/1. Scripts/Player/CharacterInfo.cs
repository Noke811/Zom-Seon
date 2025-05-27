using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    [Header("Stat")]
    public int health;
    public int stamina;
    private int _baseAtk;
    private int _baseDef;
    
    public int equipAtkBonus;
    public int equipDefBonus;
    public int FinalAtk
    {
        get
        {
            int totalAtk = _baseAtk + equipAtkBonus;
            return totalAtk;
        }
    }

    public int FinalDef
    {
        get
        {
            int totalDef = _baseDef + equipDefBonus;
            return totalDef;
        }
    }

    private void Start()
    {
        CharacterManager.Instance.Player.info = this;
    }
}