using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumalbe,
}

public enum BuffType
{
    Health,
    HealthRecovery,
    Hunger,
    Thirst,
    Stamina,
    StaminaRecovery,
    Attack,
    Speed,
}

[System.Serializable]
public class Buff
{
    [Header("Buff Info")]
    [SerializeField] BuffType type;
    public BuffType Type => type;
    [SerializeField] float value;
    public float Value => value;

    [Header("Duration")]
    [SerializeField] bool hasDuration;
    public bool HasDuration => hasDuration;
    [SerializeField] float duration;
    public float Duration => duration;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    [SerializeField] int id;
    public int Id => id;
    [SerializeField] string itemName;
    public string ItemName => itemName;
    [SerializeField] string description;
    public string Description => description;
    [SerializeField] ItemType type;
    public ItemType Type => type;
    [SerializeField] Sprite icon;
    public Sprite Icon => icon;
    [SerializeField] GameObject dropPrefab;
    public GameObject DropPrefab => dropPrefab;

    [Header("Stacking")]
    [SerializeField] bool canStack;
    public bool CanStack => canStack;
    [SerializeField] int maxAmount;
    public int MaxAmount => maxAmount;

    [Header("Buff")]
    [SerializeField] Buff[] buffs;
    public Buff[] Buffs => buffs;
}