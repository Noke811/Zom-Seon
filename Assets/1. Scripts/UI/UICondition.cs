using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition stamina;
    public Condition hunger;
    public Condition thirst;

    private void Awake()
    {
        health = transform.Find("Hp Bar").GetComponent<Condition>();
        stamina = transform.Find("Stamina Bar").GetComponent<Condition>();
        hunger = transform.Find("Hunger Bar").GetComponent<Condition>();
        thirst = transform.Find("Thirst Bar").GetComponent<Condition>();

        if (health == null)
            Debug.LogError("Health 연결 안됨");
        else
            Debug.Log("Health 연결됨");
        if (stamina == null)
            Debug.LogError("Stamina 연결 안됨");
        else
            Debug.Log("Stamina 연결됨");
        if (hunger == null)
            Debug.LogError("Hunger 연결 안됨");
        else
            Debug.Log("Hunger 연결됨");
        if (thirst == null)
            Debug.LogError("Thirst 연결 안됨");
        else
            Debug.Log("Thirst 연결됨");
    }
}