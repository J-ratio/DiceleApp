using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] screens;

    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private int index = 0;

    private void Start()
    {
        title.text = screens[index].name;

        screens[index].SetActive(true);
    }
    public void Next()
    {
        screens[index].SetActive(false);

        index++;

        if (index > screens.Length - 1) index = 0;

        title.text = screens[index].name;

        screens[index].SetActive(true);
    }
    public void Previous()
    {
        screens[index].SetActive(false);

        index--;

        if (index < 0) index = screens.Length - 1;

        title.text = screens[index].name;

        screens[index].SetActive(true);
    }
}
