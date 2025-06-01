using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float maxValue;
    public float passiveValue;
    public Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        curValue = maxValue;
    }

    public void Init()
    {
        curValue = maxValue;
    }

    private void Update()
    {
        UIBarUpdate();
    }

    private void UIBarUpdate()
    {
        slider.value = curValue / maxValue;
    }

    public void Add(float Value)
    {
        curValue = Mathf.Min(curValue + Value, maxValue);
    }

    public void Subtract(float Value)
    {
        curValue = Mathf.Max(curValue - Value, 0f);
    }
}
