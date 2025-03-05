using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSettings : MonoBehaviour
{

    [SerializeField] private Transform settingsToggleGroup;
    [SerializeField] private Transform settingsSectionGroup;

    private Toggle[] toggles;
    private GameObject[] sections;

    void Start()
    {
        toggles = settingsToggleGroup.GetComponentsInChildren<Toggle>();
        sections = new GameObject[settingsSectionGroup.childCount];

        for (int i = 0; i < sections.Length; i++)
        {
            sections[i] = settingsSectionGroup.GetChild(i).gameObject;
        }

        UpdateSettingsPage();
    }

    public void UpdateSettingsPage()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            sections[i].SetActive(toggles[i].isOn);
        }
    }
}
