using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition thirst { get { return uiCondition.thirst; } }

    private void Awake()
    {
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
        
        if(hunger.curValue > 0 && thirst.curValue > 0)
            health.Add(2f * Time.deltaTime);
        else if(hunger.curValue <= 0 && thirst.curValue <= 0)
            health.Subtract(2f * Time.deltaTime);
        else
            health.Subtract(1f * Time.deltaTime);
    }

    private void UseStamina()
    {
        if (GameManager.Instance.Player.Controller.isDash)
            stamina.Subtract(stamina.passiveValue * Time.deltaTime);
        else
            stamina.Add(stamina.passiveValue * Time.deltaTime);
        
        if (GameManager.Instance.Player.Controller.isGround() && Input.GetKeyDown(KeyCode.Space))
            stamina.Subtract(5);
    }

    public void AttackUseStamina()
    {
        stamina.Subtract(5);
    }

    public void Eat(BuffType buff, float value)
    {
        if (buff == BuffType.Health)
            health.Add(value);
        if (buff == BuffType.Stamina)
            stamina.Add(value);
        if (buff == BuffType.Hunger)
            hunger.Add(value);
        if (buff == BuffType.Thirst)
            thirst.Add(value);
    }
}