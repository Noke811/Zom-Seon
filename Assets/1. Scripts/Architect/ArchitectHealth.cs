using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class ArchitectHealth : MonoBehaviour, IDamagable, IInteractable
{
    private ArchitectData architectData;
    private int currentHealth;
    private Inventory inventory;

    public void Init(ArchitectData data)
    {
        architectData = data;
        if (architectData != null)
        {
            currentHealth = architectData.maxHealth;
        }
        else
        {
            Debug.Log("Architect data is null");
        }
        
        if (GameManager.Instance != null && GameManager.Instance.Inventory != null)
        {
            inventory = GameManager.Instance.Inventory;
        }
        else
        {
            Debug.Log("Game manager or inventory is null");
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;
        
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        // TODO: 체력 UI 업데이트
    }

    void Die()
    {
        //TODO: 파괴 효과 만들기
        Destroy(gameObject, 1f);
    }

    public string GetInteractName()
    {
        if (architectData == null) return "";
        if (currentHealth < architectData.maxHealth)
        {
            return architectData.itemName + "수리하기";
        }

        return "";
    }

    public string GetInteractDescription()
    {
        if (architectData == null) return "";

        if (currentHealth < architectData.maxHealth)
        {
            if (architectData.repairResources == null || architectData.repairResources.Length == 0)
            {
                return "수리 불가";
            }

            for (int i = 0; i < architectData.repairResources.Length; i++)
            {
                string description = "수리 필요 재료:"+ architectData.repairResources[i].id + " x" + architectData.repairResources[i].amount;
                
                if (i == architectData.repairResources.Length - 1)
                {
                    return description;
                }
                else
                {
                    return description + "\n";
                }
            }
            return "";
        }
        return "이미 최대 체력입니다.";
    }

    public void OnInteract()
    {
        if (architectData == null) return;
        if (currentHealth <= architectData.maxHealth)
        {
            TryRepair();
        }
        else
        {
            Debug.Log(architectData.itemName + "는 이미 최대 체력입니다.");
        }
    }

    void TryRepair()
    {
        if (architectData.repairResources == null || architectData.repairResources.Length == 0)
        {
            return;
        }
        
        bool canRepair = true;
        foreach (CraftingResource resource in architectData.repairResources)
        {
            if (inventory.GetResourceAmount(resource.id) < resource.amount)
            {
                canRepair = false;
                break;
            }
        }

        if (canRepair)
        {
            foreach (CraftingResource resource in architectData.repairResources)
            {
                inventory.CraftResource(resource.id, resource.amount);
            }
            currentHealth += architectData.repairAmount;
            if (currentHealth > architectData.maxHealth)
            {
                currentHealth = architectData.maxHealth;
            }
            Debug.Log("수리 완료");
        }
        else
        {
            Debug.Log("재료 부족");
        }
    }
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
