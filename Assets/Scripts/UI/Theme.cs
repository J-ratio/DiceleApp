using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Theme : MonoBehaviour
{
    [SerializeField] private Sprite darkTheme;
    [SerializeField] private Sprite sunTheme;
    [SerializeField] private Image themeImg;
    private Toggle toggle;
    private void Start()
    {
        toggle = GetComponent<Toggle>();
        Toggle_Button();
    }
    public void Toggle_Button()
    {
        if (toggle != null)
        {
            if (toggle.isOn)
            {
                themeImg.sprite = darkTheme;
            }
            else
            {
                themeImg.sprite = sunTheme;
            }
        }
    }
}
