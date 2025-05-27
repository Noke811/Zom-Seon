using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public GameObject architectMenuBase;
    public List<Button> tabButtons;
    public List<GameObject> slotBases;

    private GameObject currentSlotBase;

    void Start()
    {
        if (architectMenuBase != null)
        {
            architectMenuBase.SetActive(false);
        }

        for (int i = 0; i < tabButtons.Count; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => OnTabButtonClicked(index));
        }

        foreach (var slotBase in slotBases)
        {
            if (slotBase != null)
            {
                slotBase.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (architectMenuBase != null)
            {
                architectMenuBase.SetActive(!architectMenuBase.activeSelf);

                if (architectMenuBase.activeSelf && tabButtons.Count > 0)
                {
                    OnTabButtonClicked(0);
                }
                else
                {
                    foreach (var slotBase in slotBases)
                    {
                        if (slotBase != null)
                        {
                            slotBase.SetActive(false);
                        }
                    } 
                    currentSlotBase = null;
                }
            }
        }
    }

    void OnTabButtonClicked(int tabIndex)
    {
        if (tabIndex >= 0 && tabIndex < slotBases.Count && tabIndex < slotBases.Count)
        {
            if (currentSlotBase != null)
            {
                currentSlotBase.SetActive(false);
            }
            
            if (slotBases[tabIndex] != null)
            {
                slotBases[tabIndex].SetActive(true);
                currentSlotBase = slotBases[tabIndex];
            }
            else
            {
                currentSlotBase = null;
                Debug.Log("Tab index out of range");
            }
        }
        else
        {
            Debug.Log("Tab index out of range");
        }
    }
}
