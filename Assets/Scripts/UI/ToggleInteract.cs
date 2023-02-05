using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleInteract : MonoBehaviour
{
    private Toggle toggle;
    private Animator anime;
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        anime = GetComponent<Animator>();
        Toggle_Button(1);
        toggle.onValueChanged.AddListener(delegate
        {
            Toggle_Button(0);
        });
    }
    public void Toggle_Button(int normalTime)
    {
        if (toggle != null && anime != null)
        {
            if (toggle.isOn)
            {
                anime.Play("On", 0, normalTime);
            }
            else
            {
                anime.Play("Off", 0, normalTime);
            }
        }
    }
}
