public interface IInteractable
{
    // 상호작용 UI에 뜰 이름 반환
    public string GetInteractName();

    // 상호작용 UI에 뜰 설명 반환
    public string GetInteractDescription();

    // 상호작용 키를 눌렀을 때 실행
    public void OnInteract();
}
