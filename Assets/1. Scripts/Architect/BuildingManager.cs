using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public float gridSize = 1f;
    public LayerMask buildableLayer;
    public Material previewMaterial;
    public Material previewMaterialInvalid;
    //public AudioClip placeSound;
    //public GameObject placeEffect;
    
    private GameObject previewObject;
    public Camera mainCamera;
    private ArchitectData selectedItem;
    private Vector3Int currentGridPosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
        UpdatePreview();
    }

    void HandleInput()
    {
        if (selectedItem == null)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
        else if (selectedItem.isPlaceable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                RotatePreviewObject();
            }
        }
        else if (selectedItem.isTool)
        {
            TryCraftTool();
        }
    }
    void UpdatePreview()
    {
        if (selectedItem == null || !selectedItem.isPlaceable || previewObject == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildableLayer))
        {
            currentGridPosition = GetGridPosition(hit.point);
            previewObject.transform.position = currentGridPosition;
            bool canBuild = CanPlaceObject(currentGridPosition);
            SetPreviewMaterial(canBuild);
        }
        else
        {
            previewObject.transform.position = ray.GetPoint(100f);
            SetPreviewMaterial(false);
        }
    }
    Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        return new Vector3Int(
            Mathf.RoundToInt(worldPosition.x / gridSize) * (int)gridSize,
            Mathf.RoundToInt(worldPosition.y / gridSize) * (int)gridSize,
            Mathf.RoundToInt(worldPosition.z / gridSize) * (int)gridSize
        );
    }
    bool CanPlaceObject(Vector3Int position)
    {
        Collider[] colliders = Physics.OverlapBox(position, previewObject.GetComponent<Collider>().bounds.size / 2f, previewObject.transform.rotation, buildableLayer);
        if (colliders.Length > 0) return false;
        
        if (selectedItem.isFoundation)
        {
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out hit, 1f, buildableLayer))
            {
                if (!hit.collider.CompareTag("Ground")) return false;
            }
            else
            {
                return false;
            }
        }
        else if (selectedItem.canBuildableTags.Count > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out hit, 1f, buildableLayer))
            {
                if (!selectedItem.canBuildableTags.Contains(hit.collider.tag) && !hit.collider.CompareTag("Foundation")) return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    void TryPlaceObject()
    {
        if (selectedItem == null || !selectedItem.isPlaceable || !CanPlaceObject(currentGridPosition)) return;

        GameObject placedObject = Instantiate(selectedItem.itemPrefab, currentGridPosition, previewObject.transform.rotation);
        
        CancelPlacement();
    }

    void CancelPlacement()
    {
        Destroy(previewObject);
        previewObject = null;
        selectedItem = null;
    }

    public void HandleItemSelected(ArchitectData item)
    {
        selectedItem = item;
        if (selectedItem.isPlaceable)
        {
            ShowPreview();
        }
        else if (selectedItem.isTool)
        {
            TryCraftTool();
        }
        else
        {
            Debug.Log("선택한 아이템은 설치하거나 제작할 수 없습니다.");
            selectedItem = null;
        }
    }
    void ShowPreview()
    {
        Destroy(previewObject);
        previewObject = Instantiate(selectedItem.itemPrefab, Vector3.zero, Quaternion.identity);
        SetPreviewMaterial(true);
    }

    void SetPreviewMaterial(bool isValid)
    {
        Material mat = isValid ? previewMaterial : previewMaterialInvalid;
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material = mat;
        }
    }

    void TryCraftTool()
    {
        if (selectedItem != null && selectedItem.isTool)
        {
            if (HasMaterials())
            {
                ConsumeMaterials();
                Debug.Log(selectedItem.itemName + " 제작 성공!");
            }
            else
            {
                Debug.Log("재료가 부족합니다.");
            }
            selectedItem = null;
        }
    }

    void RotatePreviewObject()
    {
        if (previewObject != null)
        {
            previewObject.transform.Rotate(Vector3.up, 90f);
        }
    }
    bool HasMaterials()
    {
        //인벤토리 연동
        return true;
    }

    void ConsumeMaterials()
    {
        //인벤토리 연동
        Debug.Log("재료 소모");
    }
}
