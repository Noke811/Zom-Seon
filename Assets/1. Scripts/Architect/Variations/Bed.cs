using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IInteractable
{
    public string GetInteractName()
    {
        if (GameManager.Instance != null && GameManager.Instance.DayCycle != null)
        {
            if (GameManager.Instance.DayCycle.IsNight)
            {
                return "잠자기";
            }
            else
            {
                return "";
            }
        }
        return "";
    }

    public string GetInteractDescription()
    {
        if (GameManager.Instance != null && GameManager.Instance.DayCycle != null)
        {
            if (GameManager.Instance.DayCycle.IsNight)
            {
                return "밤을 건너뛰고 아침으로 시간을 변경합니다.";
            }
            else
            {
                return "지금은 잠을 잘 수 없습니다.";
            }
        }
        return "";
    }
    public void OnInteract()
    {
        if (GameManager.Instance != null && GameManager.Instance.DayCycle != null)
        {
            if (GameManager.Instance.DayCycle.IsNight)
            {
                GameManager.Instance.DayCycle.SetTimeToMorning();
                // TODO: 화면 페이드 아웃
            }
            else
            {
                Debug.Log("낮에는 침대를 사용할 수 없습니다.");
            }
        }
    }
}
