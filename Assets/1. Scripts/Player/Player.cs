using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public CharacterInfo Info { get; private set; }
    public Equipment Equipment { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Info = GetComponent<CharacterInfo>();
        Equipment = GetComponent<Equipment>();
        Cursor.lockState = CursorLockMode.Locked;
    }
}