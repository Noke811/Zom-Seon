using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    public LayerMask buildableLayer; // 건축 가능한 지면 레이어
    public LayerMask obstacleLayer; // 건축 불가능한 레이어

    public Material previewMaterial; // 배치 가능할 때의 Material
    public Material previewMaterialInvalid; // 배치 불가능할 때의 Material

    public float buildableDistance = 10f;

    private Camera mainCamera;
    private ArchitectData selectedItem;
    private GameObject previewObject;
    private Vector3 currentPosition;
    private Bounds previewBounds;

    private bool isBuildingMode = false; // 건설 모드
    private bool canPlace = false; // 배치 가능한지 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(!isBuildingMode) // 건설 모드가 아니면 실행하지 않음.
        {
            return;
        }

        UpdatePreviewPosition();
        HandleRotationInput();
        HandlePlacementInput();
        HandleCancelInput();
    }

    public void HandleItemSelected(ArchitectData item)
    {
        if (isBuildingMode)
        {
            CancelPlacement();
        }
        
        selectedItem = item;

        if (selectedItem == null) return;

        if (selectedItem.isPlaceable)
        {
            // 건설 모드 시작
            isBuildingMode = true;
            ShowPreview();
        }
        else if (selectedItem.isTool)
        {
            // 도구 제작
            TryCraftTool();
        }
        else
        {
            selectedItem = null;
        }
    }

    private void UpdatePreviewPosition()
    {
        if (previewObject == null) return;
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, buildableDistance, buildableLayer))
        {
            previewObject.SetActive(true);
            currentPosition = hit.point;
            previewObject.transform.position = currentPosition;

            CheckIfCanPlace();
        }
        else
        {
            previewObject.SetActive(false);
            canPlace = false;
        }
    }

    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotatePreviewObject();
        }
    }

    private void HandlePlacementInput()
    {
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            TryPlaceObject();
        }
    }

    private void HandleCancelInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelPlacement();
        }
    }

    private void ShowPreview()
    {
        // 아이템의 미리보기를 생성합니다.
        if (selectedItem == null || selectedItem.itemPreviewPrefab == null) return;
        
        previewObject = Instantiate(selectedItem.itemPreviewPrefab, Vector3.zero, Quaternion.identity);
        
        Collider previewCollider = previewObject.GetComponent<Collider>();
        if (previewCollider != null)
        {
            previewCollider.enabled = false;
        }
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            previewBounds = new Bounds(renderers[0].bounds.center, Vector3.zero);
            foreach (Renderer renderer in renderers)
            {
                previewBounds.Encapsulate(renderer.bounds);
            }
        }
    }

    private void RotatePreviewObject()
    {
        if (previewObject == null) return;
        
        previewObject.transform.Rotate(Vector3.up, 45f);
        CheckIfCanPlace();
    }

    private void CheckIfCanPlace()
    {
        canPlace = CanPlaceObject();
        SetPreviewMaterial(canPlace);
    }

    private void TryPlaceObject()
    {        
        // 실제 건축물을 배치하는 로직
        if (!HasMaterials())
        {
            Debug.Log("Not enough materials");
            // UI제공 
            return;
        }

        if (!canPlace)
        {
            Debug.Log("Can't place object");
            return;
        }
        
        GameObject newObject = Instantiate(selectedItem.itemPrefab, currentPosition, previewObject.transform.rotation);
        newObject.layer = LayerMask.NameToLayer("Foundation"); //TODO: 예시 레이어. 이후 변경 필요
        ConsumeMaterials();
        Debug.Log(selectedItem.itemName + "건설 완료");
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        // 건설모드 종료
        isBuildingMode = false;
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewObject = null;
        selectedItem = null;
        canPlace = false;
    }

    private bool CanPlaceObject()
    {
        // 다른 오브젝트들의 레이어와의 충돌을 확인하여 건설 가능 여부를 반환.
        return !Physics.CheckBox(currentPosition, previewBounds.extents, previewObject.transform.rotation, obstacleLayer);
    }

    private void SetPreviewMaterial(bool isValid)
    {
        // 배치 가능 여부에 따라 미리보기 오브젝트의 Material을 변경합니다.
        if (previewObject == null) return;
        
        Material newMaterial = isValid ? previewMaterial : previewMaterialInvalid;
        foreach (var renderer in previewObject.GetComponentsInChildren<Renderer>())
        {
            renderer.material = newMaterial;
        }
    }

    private void TryCraftTool()
    {
        // 도구 제작 시도
        if (HasMaterials())
        {
            ConsumeMaterials();
            Vector3 spwanPos = GameManager.Instance.Player.Head.transform.position + GameManager.Instance.Player.Head.transform.forward;
            Instantiate(selectedItem.itemPrefab, spwanPos, Quaternion.identity);
        }
        else
        {
            Debug.Log("Not enough materials");
        }
        selectedItem = null;
    }

    private bool HasMaterials()
    {
        foreach(CraftingResource resource in selectedItem.craftingResources)
        {
            if (resource.amount > GameManager.Instance.Inventory.GetResourceAmount(resource.id))
            {
                return false;
            }
        }
        return true;
    }

    private void ConsumeMaterials()
    {
        foreach (CraftingResource resource in selectedItem.craftingResources)
        {
            GameManager.Instance.Inventory.CraftResource(resource.id, resource.amount);
        }
        
        Debug.Log("Consume materials");
    }
}