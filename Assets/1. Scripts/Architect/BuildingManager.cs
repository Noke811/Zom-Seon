using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public LayerMask buildableLayer;
    public Material previewMaterial;
    public Material previewMaterialInvalid;
    
    private GameObject previewObject;
    public Camera mainCamera;
    private ArchitectData selectedItem;
    private Vector3 currentPosition;
    
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
            currentPosition = hit.point;
            previewObject.transform.position = currentPosition;
            bool canBuild = CanPlaceObject(currentPosition);
            Debug.Log("Raycast Hit: " + hit.collider.gameObject.name);
            SetPreviewMaterial(canBuild);
        }
        else
        {
            previewObject.transform.position = ray.GetPoint(100f);
            SetPreviewMaterial(false);
        }
    }
    bool CanPlaceObject(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, previewObject.GetComponent<Collider>().bounds.size / 2f, previewObject.transform.rotation, buildableLayer);
        Debug.Log("Colliders Count: " + colliders.Length);
        return colliders.Length <= 1;
    }
    void TryPlaceObject()
    {
        if (selectedItem == null || !selectedItem.isPlaceable || !CanPlaceObject(currentPosition)) return;

        GameObject placedObject = Instantiate(selectedItem.itemPrefab, currentPosition, previewObject.transform.rotation);

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
