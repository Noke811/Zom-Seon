using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialCost
{
    public ItemData item;
    public int amount;
}
[CreateAssetMenu(fileName = "Architect", menuName = "new Architect")]
public class ArchitectData : ScriptableObject
{
    // interface IDamageable 적용 필요
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemPrefab;
    public GameObject itemPreviewPrefab;
    public List<MaterialCost> materialCosts;
    public bool isPlaceable;
    public bool isTool;
    public Material originalMaterial;
}
