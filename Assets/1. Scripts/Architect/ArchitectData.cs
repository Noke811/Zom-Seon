using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Architect", menuName = "new Architect")]
public class ArchitectData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemPrefab;
    public Dictionary<string, int> requiredMaterials;
    public bool isPlaceable;
    public bool isTool;
    public Material originalMaterial;
}
