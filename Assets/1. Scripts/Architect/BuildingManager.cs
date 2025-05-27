using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public float gridsize = 1f;
    public LayerMask BuildableLayer;
    public Material previewMaterial;
    public Material previewMaterialInvalid;
    
    private GameObject previewObject;
    public Camera mainCamera;
    private ItemData selectedItem;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (selectedItem != null && selectedItem.isPlaceable)
        {
            UpdatePreviewObjectPosition();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceObject();
            }
        }
    }

    public void HandleItemSelected(ItemData item)
    {
        selectedItem = item;

        if (selectedItem.isPlaceable)
        {
            ShowPreviewObject();
        }
        else if (selectedItem.isTool)
        {
            TryCraftTool();
        }
        else
        {
            Debug.Log("선택한 아이템은 설치나 제작이 불가능합니다.");
            selectedItem = null;
        }
    }

    void ShowPreviewObject()
    {
        DestroyPreviewObject();
        if (selectedItem != null && selectedItem.itemPrefab != null && selectedItem.isPlaceable)
        {
            previewObject = Instantiate(selectedItem.itemPrefab, Vector3.zero, Quaternion.identity);
            SetPreviewMaterial(true);
        }
        else
        {
            Debug.Log("설치 불가");
        }
    }

    void DestroyPreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }
    }

    void UpdatePreviewObjectPosition()
    {
        if (previewObject != null && selectedItem != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, BuildableLayer))
            {
                Vector3Int gridPosition = GetGridPosition(hit.point);
                previewObject.transform.position = gridPosition;

                bool canBuild = true;
                
                if (selectedItem.canBuildableTags.Count > 0 && !selectedItem.canBuildableTags.Contains(hit.collider.tag))
                    canBuild = false;
                SetPreviewMaterial(canBuild);
            }
            else
            {
                previewObject.transform.position = ray.GetPoint(100f);
                SetPreviewMaterial(false);
            }
        }
    }

    Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        return new Vector3Int(
            Mathf.RoundToInt(worldPosition.x / gridsize) * (int)gridsize,
            Mathf.RoundToInt(worldPosition.y / gridsize) * (int)gridsize,
            Mathf.RoundToInt(worldPosition.z / gridsize) * (int)gridsize
        );
    }

    void TryPlaceObject()
    {
        if (previewObject != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, BuildableLayer))
            {
                bool canBuild = true;
                
                if (selectedItem.canBuildableTags.Count > 0 && !selectedItem.canBuildableTags.Contains(hit.collider.tag))
                    canBuild = false;
                if (canBuild)
                {
                    Instantiate(previewObject, hit.point, Quaternion.identity);
                    DestroyPreviewObject();
                }
                else
                {
                    {
                        Debug.Log("설치 불가");
                    }
                }
            }
        }
    }

    void TryCraftTool()
    {
        if (selectedItem != null && selectedItem.isTool)
        {
            // 인벤토리 추가 필요
            Debug.Log(selectedItem.itemName + "제작성공");
            selectedItem = null;
        }
        else
        {
            //재료 부족 시
            Debug.Log("제작 실패");
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

    void SetPreviewMaterial(bool isValid)
    {
        if (previewObject != null && selectedItem != null)
        {
            Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (isValid)
                {
                    renderer.material = previewMaterial;
                }
                else
                {
                    renderer.material = previewMaterialInvalid;
                }
            }
        }
    }
}
