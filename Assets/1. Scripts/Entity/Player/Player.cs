using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public PlayerCondition Condition { get; private set; }
    public Inventory Inventory { get; private set; }
    public CharacterStat Stat { get; private set; }
    public Equipment Equipment { get; private set; }
    public InteractableDetector Head { get; private set; }
    public DamagableDetector Detector { get; private set; }

    public Vector3 spawnPoint = new Vector3(0f, -0.01f, 0);
    
    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Condition = GetComponent<PlayerCondition>();
        Inventory = GetComponent<Inventory>();
        Stat = GetComponent<CharacterStat>();
        Equipment = GetComponent<Equipment>();
        Head = GetComponentInChildren<InteractableDetector>();
        Detector = Head.GetComponentInChildren<DamagableDetector>();
    }
}