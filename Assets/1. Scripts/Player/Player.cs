using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public Inventory Inventory { get; private set; }
    public CharacterInfo Info { get; private set; }
    public Equipment Equipment { get; private set; }
    public DamagableDetector Detector { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        Inventory = GetComponent<Inventory>();
        Info = GetComponent<CharacterInfo>();
        Equipment = GetComponent<Equipment>();
        Detector = GetComponentInChildren<DamagableDetector>();
    }
}