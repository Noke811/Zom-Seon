using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemPrefab;
    public Dictionary<string, int> requiredMaterials;
    public bool isPlaceable;
    public List<string> canBuildableTags;
    public bool isTool;
}
