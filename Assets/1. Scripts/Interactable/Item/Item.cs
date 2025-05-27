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
        
    }
}
