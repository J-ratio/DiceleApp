using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ShineEffect : MonoBehaviour
{
    [SerializeField] private UnityEvent startAnimation;
    [SerializeField] private UnityEvent endAnimation;
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private Vector2 endPoint;
    [SerializeField] private float time = 2;
    [SerializeField] private float startdelay;
    [SerializeField] private float delay;
    private RectTransform rectTransform;
    float eTime;
    float distance;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        distance = Vector2.Distance(startPoint,endPoint);
    }

    private void OnEnable()
    {
        rectTransform.anchoredPosition = startPoint;
        startAnimation?.Invoke();
    }
    
    private void OnDisable()
    {
        StopCoroutine("UpdateAnime");
    }

    public void StartAnimation()
    {
        rectTransform.anchoredPosition = startPoint;
        StartCoroutine("UpdateAnime");
    }

    IEnumerator UpdateAnime()
    {
        yield return new WaitForSeconds(startdelay);

        //eTime = 0;
        while (rectTransform.anchoredPosition != endPoint)
        {
            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, endPoint, Time.deltaTime * (distance/time));

            //eTime += Time.deltaTime/time;

            if (rectTransform.anchoredPosition == endPoint)
            {
                yield return new WaitForSeconds(delay);
                endAnimation?.Invoke();
            }
            yield return null;
        }
    }
}
