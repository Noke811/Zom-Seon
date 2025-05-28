using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float maxValue;
    public float passiveValue;
    public Image uiBar;

    private void Start()
    {
        curValue = maxValue;
    }

    private void Update()
    {
        UIBarUpdate();
    }

    private void UIBarUpdate()
    {
        uiBar.fillAmount = curValue / maxValue;
    }
}