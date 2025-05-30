using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public CharacterInfo Info { get; private set; }
    public Equipment Equipment { get; private set; }
    public InteractableDetector Head { get; private set; }
    public DamagableDetector Detector { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Info = GetComponent<CharacterInfo>();
        Equipment = GetComponent<Equipment>();
        Head = GetComponentInChildren<InteractableDetector>();
        Detector = Head.GetComponentInChildren<DamagableDetector>();
    }
}