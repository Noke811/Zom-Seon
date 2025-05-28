using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonHandler : MonoBehaviour
{
    protected Button[] buttons;
    
    // 버튼 초기화
    public virtual void Init()
    {
        buttons = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            BindButton(i);
        }

        HideButtons();
    }

    // 모든 버튼 숨기기
    public void HideButtons()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    // 모든 버튼 띄우기
    public void ShowButtons()
    {
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }

    // 해당되는 버튼을 눌렀을 때 실행
    protected abstract void OnButtonClick(int index);

    // 버튼 연결
    protected void BindButton(int index)
    {
        buttons[index].onClick.AddListener(() => OnButtonClick(index));
    }
}
