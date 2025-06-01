using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] ItemData data;

    public string GetInteractName()
    {
        return data.ItemName;
    }

    public string GetInteractDescription()
    {
        return data.Description;
    }

    public void OnInteract()
    {
        bool canSaveItem = GameManager.Instance.Inventory.AddInventory(data, 1);

        if (!canSaveItem)
        {
            Debug.Log("인벤토리가 가득 찼음!");
            return;
        }
        Destroy(gameObject);
    }
}
