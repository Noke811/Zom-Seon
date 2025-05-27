using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Player _player;
    private Animator _anim;
    public bool isRun;
    public bool isJump;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _anim = GetComponent<Animator>();
    }
}