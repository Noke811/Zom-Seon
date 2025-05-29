using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;
    public PlayerController controller;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        GameObject playerInfo = GameObject.Find("PlayerInfo");
        uiCondition = playerInfo.GetComponent<UICondition>();
    }

    public void TakeDamage(int damage)
    {
        health.Subtract(damage);
    }

    private void Update()
    {
        PassiveUpdate();
        UseStamina();
    }

    private void PassiveUpdate()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        thirst.Subtract(thirst.passiveValue * Time.deltaTime);
        if (hunger.curValue > 0f)
            health.Add(health.passiveValue);
        else
            health.Subtract(hunger.passiveValue);
        if (thirst.curValue > 0f)
            health.Add(health.passiveValue);
        else
            health.Subtract(thirst.passiveValue);
    }

    private void UseStamina()
    {
        if (controller.isDash)
            stamina.Subtract(stamina.passiveValue * Time.deltaTime);
        else
            stamina.Add(stamina.passiveValue * Time.deltaTime);
    }
}