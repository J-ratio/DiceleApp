using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitSlider : MonoBehaviour
{
    public Slider slider;
    public GameObject logo;


    // Start is called before the first frame update
    void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = 0;
    }

    void OnEnable()
    {
        StartCoroutine("Slide");
    }

    public void StartSlider()
    {
        transform.parent.gameObject.SetActive(true);
        slider.value = 0;
        StartCoroutine("Slide");
    }


    IEnumerator Slide()
    {
        yield return new WaitForSeconds(1.0f);
        
        logo.SetActive(false);

        while(slider.value < 0.4f)
        {
            slider.value += 0.03f;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.5f);

        while(slider.value < 0.8f)
        {
            slider.value += 0.03f;
            yield return new WaitForSeconds(0.01f);
        }

        yield break;
    }


    public void SetSliderToFull()
    {
        StartCoroutine("Full");
    }

    IEnumerator Full()
    {
        while(slider.value < 1f)
        {
            slider.value += 0.01f;
            yield return new WaitForSeconds(0.05f);
        }

        transform.parent.gameObject.SetActive(false);
    }


    void OnDisable()
    {
        slider.value = 0;
        StopCoroutine("Full");
    }
}
