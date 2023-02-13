using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultBubble : MonoBehaviour
{
    [SerializeField] private GameObject bubble;
    private bool clicking;
    public void Click()
    {
        StartCoroutine(BubbleAnimation());
    }

    private IEnumerator BubbleAnimation()
    {
        if (!clicking)
        {
            clicking = true;
            bubble.SetActive(true);
            yield return new WaitForSeconds(3);
            clicking = false;
            bubble.SetActive(false);
        }
    }
}


