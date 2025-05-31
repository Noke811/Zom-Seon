# 건축 (Architect)
---
## Architect 스크립트에 대한 설명입니다.

### TabManager.cs
- TabManager 스크립트는 건축 메뉴 시스템의 동작을 처리합니다.
  - `architectMenyBase` : 건축 메뉴 UI의 오브젝트입니다.
  - `tabButtons` : 탭으로 작동하는 UI 버튼 목록입니다.
  - `slotBases` : 각 탭에 해당하는 목록을 나타내는 오브젝트입니다.
  - `isActive` : `architectMenyBase`가 현재 씬에서 활성화되어 있는지 여부를 나타내는 속성입니다.
  - `Start()` : 초기에 `architectMenuBase`를 비활성화하고, `tabButtons`를 순회하며 각 버튼의 `onClick`이벤트에 리스터를 할당하여 해당 탭 목록과 함께 `OnTabButtonClicked()`를 호출하도록 합니다. 또한 모든 `slotBases`가 초기에 비활성화됩니다.
  - `SetMenu()` : `architectMenuBase`를 활성화/비활성화 합니다. UI가 활성화될 경우, 기본적으로 첫번째 탭의 목록을 표시하고, 메뉴가 비활성화될 경우 모든 `slotBases`를 숨기고 `currentSlotBase`를 지웁니다.
  -  `OnTabButtonClicked` : 탭 버튼을 클릭하면 호출되면 메서드입니다. 이전에 활성화된 목록을 비활성화하고 클릭한 메뉴의 목록을 활성화합니다.
### Slot.cs
- Slot 스크립트는 UI 내의 개별 아이템 슬롯의 동작을 처리합니다.
  - `itemData` : 데이터를 관리하는 `ArchitectData(ScriptableObject)`.
  - `slotImage` : 아이템의 아이콘을 표시하는 데에 사용됩니다.
  - `slotText` : 아이템의 이름을 표시하는 데에 사용되는 컴포넌트입니다.
  - `slotButton` : 플레이어가 상호작용할 수 있도록 하는 Button 컴포넌트 입니다.
  - `parentSlotBase` : 이 슬롯을 관리하는 `SlotBase`스크립트에 대한 참조입니다.
  - `Start()` : `slotButton`의 `onClick`이벤트에 리스너를 추가하여 버튼을 누르면 `OnSlotClicked()`를 호출합니다. 또한 `UpdateSlotUI()`를 호출하여 슬롯의 모양을 초기화합니다.
  - `UpdateSlotUI()` : 슬롯을 업데이트합니다. `slotImage`와 `slotText`를 설정합니다.
  - `OnSlotClicked()` : 슬롯의 버튼을 클릭하면 호출되는 메서드입니다. `SlotBase`의 `OnSlotSelected()` 메서드를 호출하여 `itemData`를 전달합니다.
### SlotBase.cs
- SlotBase 스크립트는 `Slot`을 관리합니다. 아이템 데이터 목록을 가져와서 슬롯을 만들고 표시하는 역할을 합니다.
  - `slotPrefab` : 개별 슬롯의 UI요소를 생성하는 데에 사용되는 프리팹입니다.
  - `itemDatas` : 슬롯에 표시될 `ArchitectData` 목록입니다.
  - `OnEnable()` : 오브젝트가 활성화될 때 호출되는 메서드입니다. `CreateSlots()`메서드를 호출합  니다.
  - `OnDisable()` : 오브젝트가 비활성화될 때 호출되는 메서드입니다. 생성된 모든 `createdSlots`를 순회하여 해당 오브젝트를 파괴하여 UI를 정리합니다.
  - `CreateSlots()` : `itemDatas` 목록을 기반으로 `Slot` 오브젝트를 생성합니다. 목록의 각 `ArchitectData`에 대해 `slotPrefab`을 사용하여 새 슬롯을 만듭니다. 슬롯의 각 정보를 할당합니다.
  - `OnSlotSelected` : 슬롯이 클릭될 때 호출되는 메서드입니다. 씬에서 `BuildingManager` 오브젝트를 찾아서 `HandleItemSelected()` 메서드를 호출합니다.
 
### ArchitectData.cs
- ArchitectData는 건축 또는 제작할 수 있는 아이템의 데이터를 저장하는 ScriptableObject입니다. 이를 통해 다양한 아이템을 쉽게 만들고 관리할 수 있습니다.
  - `CraftingResource` : 제작을 위한 자원 요구사항을 정의하는 데에 사용되는 클래스입니다. 인스펙터에서 조정가능합니다.
  - `itemName` : 아이템의 이름입니다.
  - `itemSprite` : UI에서 아이템을 나타내는 데에 사용되는 2D 이미지입니다.
  - `itemPrefab` : 아이템을 건축하거나 제작할 때 실제로 생성되는 오브젝트 프리팹입니다.
  - `itemPreviewPrefab` : 배치 시도 시 보이기 위해 사용되는 프리팹입니다.
  - `isPlaceable` : 배치를 위한 아이템인지 여부를 나타냅니다.
  - `isTool` : 도구인지 여부를 나타냅니다.
  - `craftingResources` : 이 아이템을 제작하거나 건축하는 데에 필요한 자원을 정의하는 배열입니다.
  - `maxHealth` : 아이템의 내구도입니다.
  - `repairResources` : 아이템을 수리하는 데에 필요한 자원을 정의하는 배열입니다.
  - `repairAmount` : 아이템을 수리할 때 회복되는 체력의 양입니다.
 
### ArchitectHealth.cs
- ArchitectHealth 스크립트는 `ArchitectData`에서 생성된 건축 가능한 구조물의 체력 및 수리 메커니즘을 관리합니다. `IDamagable` 및 `IInteractable` 인터페이스를 구현합니다.
  - `Init` : 제공된 `ArchitectData`로 컴포넌트를 초기화합니다. `currentHealth`를 `architectData.maxHealth`로 설정하고 `GameManager`에서 `Inventory`에 대한 참조를 가져오려고 시도합니다.
  - `TakeDamage` :` currentHealth`를 `damage` 양만큼 줄입니다. 체력이 0 이하로 떨어지면 `Die()` 메서드를 호출합니다.
  - `Die()` : 오브젝트 파괴를 처리합니다. 1초 지연 후 오브젝트를 파괴합니다.
  - `GetInteractName()` : 플레이어가 오브젝트와 상호작용할 수 있을 때 UI 표시에 사용할 문자열을 반환합니다. 오브젝트의 체력이 최대 체력보다 낮으면 아이템 이름에 "수리하기"를 연결하여 반환합니다. 그렇지 않으면 빈 문자열을 반환합니다.
  - `GetInteractDescription()` : 상호작용에 대한 설명을 제공합니다. 오브젝트를 수리해야 하는 경우 필요한 `repairResources`를 나열합니다. 자원이 필요 없거나 최대 체력이면 적절한 메시지("수리 불가", "이미 최대 체력입니다.")를 제공합니다.
  - `OnInteract()` : 플레이어가 오브젝트와 상호작용할 때 호출됩니다. `currentHealth`가 `architectData.maxHealth`보다 작으면 `TryRepair()`를 호출합니다. 그렇지 않으면 아이템이 이미 최대 체력임을 나타내는 로그를 호출합니다.
  - `TryRepair()` : 플레이어가 `inventory`에 필요한 `repairResources`를 가지고 있는지 확인합니다. 충분한 자원이 있으면 `inventory.CraftResource()`를 사용하여 자원을 소비하고 `currentHealth`를 `architectData.repairAmount`만큼 증가시키고, "수리 완료" 메시지를 기록합니다. 자원이 부족하면 "재료 부족" 메시지를 기록합니다.
 
### BuildingManager.cs
- BuildingManager 스크립트는 `ArchitectData`에 의해 정의된 아이템 배치 및 제작 로직을 처리하는 싱글톤입니다.
  - `buildableLayer` : 오브젝트를 배치할 수 있는 표면을 정의하는 레이어 마스크입니다.
  - `obstacleLayer` : 배치를 방해하는 오브젝트를 정의하는 레이어 마스크입니다.
  - `previewMaterial` : 배치가 유효할 때 미리보기 오브젝트에 적용되는 머티리얼입니다.
  - `previewMaterialInvalid` : 배치가 유효하지 않을 때 미리보기 오브젝트에 적용되는 머티리얼입니다.
  - `buildableDistance` : 카메라에서 오브젝트를 배치할 수 있는 최대 거리입니다.
  - `Start()` : 메 카메라를 가져옵니다.
  - `Update()` : `isBuildingMode`가 아니면 아무 작업도 수행하지 않습니다. 그렇지 않으면 미리보기 오브젝트의 위치를 업데이트하고(`UpdatePreviewPosition()`), 회전 입력(`HandleRotationInput()`), 배치 입력(`HandlePlacementInput()`) 및 취소 입력(`HandleCancelInput()`)을 처리하는 메서드를 호출합니다.
  - `HandleItemSelected` : 아이템이 선택될 때 호출되는 메서드입니다. 이미 건설 모드인 경우 현재 배치를 취소합니다. `selectedItem`을 설정합니다. 아이템이 `isPlaceable`이면 건설 모드를 시작하고 미리보기를 표시합니다. `isTool`이면 도구 제작을 시도합니다.
  - `UpdatePreviewPosition()` : 마우스 위치에서 `buildableLayer` 로 레이캐스트를 수행합니다. 적중하면 `previewObject`를 활성화하고 적중 지점에 배치한 다음 `CheckIfCanPlace()`를 호출합니다. 적중하지 않으면 `previewObject`를 비활성화하고 `canPlace`를 `false`로 설정합니다.
  - `HandleRotationInput()` : 'R' 키를 누르면 `RotatePreviewObject()`를 호출합니다.
  - `HandlePlacementInput()` : 마우스를 클릭하면 `canPlace`가 `true`일  `TryPlaceObject()`를 호출합니다.
  - `HandleCancelInput()` : `Esc` 키를 누르면 `CancelPlacement()`를 호출합니다.
  - `ShowPreview()` : `selectedItem` 과 연관된 `itemPreviewPrefab`을 인스턴스화합니다. 미리보기 오브젝트의 콜라이더를 비활성화하고 렌더러를 기반으로 경계를 계산합니다.
  - `RotatePreviewObject()` : `previewObject`를 Y축을 중심으로 45도 회전한 다음 `CheckIfCanPlace()`를 사용하여 배치 유효성을 다시 확인합니다.
  - `CheckIfCanPlace()` : `CanPlaceObject()`를 호출하여 현재 위치 및 회전에서 오브젝트를 배치할 수 있는지 확인한 다음 `SetPreviewMaterial()`을 사용하여 미리보기 머티리얼을 업데이트합니다.
  - `TryPlaceObject()` : 플레이어가 재료를 가지고 있고 `canPlace`가 `true`인지 확인합니다. 둘 다 `true`이면 `currentPosition`에 `previewObject`의 회전으로 실제 `selectedItem.itemPrefab`을 생성합니다. 새 오브젝트의 레이어를 설정하고, 재료를 소비하고, 존재하는 경우 `ArchitectHealth` 컴포넌트를 초기화한 다음 배치 모드를 취소합니다. 재료가 부족하거나 배치할 수 없는 경우 메시지를 기록합니다.
  - `CancelPlacement()` : 건설 모드를 종료하고 `previewObject`를 파괴하며 `selectedItem` 및 `canPlace`를 재설정합니다.
  - `CanPlaceObject()` : `Physics.CheckBox()`를 사용하여 `previewObject`(현재 위치, 회전 및 경계에서)가 `obstacleLayer`의 콜라이더와 겹치는지 확인합니다. 겹치지 않으면(배치 가능) `true`를 반환합니다.
  - `SetPreviewMaterial` : `previewObject`의 모든 렌더러 머티리얼을 `isValid`가 `true`이면 `previewMaterial`로, `isValid`가 `false`이면 `previewMaterialInvalid`로 변경합니다.
  - `TryCraftTool()` : 아이템이 도구인 경우 이 메서드는 플레이어가 `HasMaterials()`를 가지고 있는지 확인합니다. 그렇다면 재료를 소비하고 플레이어 머리 앞에 `selectedItem.itemPrefab`을 인스턴스화합니다. 그런 다음 `selectedItem`을 재설정합니다.
  - `HasMaterials()` : `selectedItem.craftingResources`를 순회하며 `GameManager.Instance.Inventory`에 각 필수 자원이 충분한 양이 있는지 확인합니다. 자원이 부족하면 `false`를 반환합니다.
  - `ConsumeMaterials()` : `selectedItem.craftingResources`를 순회하며 각 자원에 대해 `GameManager.Instance.Inventory.CraftResource()`를 호출하여 플레이어 인벤토리에서 양을 차감합니다. 재료가 소비되었음을 나타내는 메시지를 기록합니다.
