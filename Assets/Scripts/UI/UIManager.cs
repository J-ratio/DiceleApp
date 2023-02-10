using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] screens;

    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private int index = 0;

    private int currentScreenIndex;

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
        currentScreenIndex = index;
    }
    public void Previous()
    {
        screens[index].SetActive(false);

        index--;

        if (index < 0) index = screens.Length - 1;

        title.text = screens[index].name;

        screens[index].SetActive(true);
        currentScreenIndex = index;
    }



    public void ShowScreen(string name)
    {
        int temp = 0;

        foreach(GameObject screen in screens)
        {
            //Debug.Log(screen.name);
            if(screen.name == name)
            {
                screen.SetActive(true);
                screens[currentScreenIndex].SetActive(false);
                currentScreenIndex = temp;
                break;
            }
            temp++;
        }
    }
    
}
