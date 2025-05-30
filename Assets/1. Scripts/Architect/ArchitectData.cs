using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraftingResource
{
    public int id;
    public int amount;
}

[CreateAssetMenu(fileName = "Architect", menuName = "new Architect")]
public class ArchitectData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemPrefab;
    public GameObject itemPreviewPrefab;
    public bool isPlaceable;
    public bool isTool;
    public CraftingResource[] craftingResources;

    [Header("Health & Repair")]
    public int maxHealth;
    public CraftingResource[] repairResources;
    public int repairAmount;
}
