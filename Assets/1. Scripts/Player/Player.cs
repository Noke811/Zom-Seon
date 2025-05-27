using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public CharacterInfo info;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        info = GetComponent<CharacterInfo>();
    }
}