using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Medals : PSTBase
{
    [SerializeField] private Animator dailyMedalsSelf;
    [SerializeField] private Animator weeklyMedalsSelf;
    [SerializeField] private Image dailyMedalsImage;
    [SerializeField] private Image weeklyMedalsImage;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color nonActiveColor;

    private int medalIndex = 0;//0=>Daily,1=>Weekly
    private WaitForSeconds delay = new WaitForSeconds(0.5f);

    // Start is called before the first frame update
    void OnEnable()
    {
        if (medalIndex == 0)
        {
            dailyMedalsImage.color = activeColor;
            weeklyMedalsImage.color = nonActiveColor;
            dailyMedalsSelf.Play("Open_MedalSelf", 0, 1);
            weeklyMedalsSelf.Play("Close_MedalSelf", 0, 1);
        }
        else
        {
            dailyMedalsImage.color = nonActiveColor;
            weeklyMedalsImage.color = activeColor;
            dailyMedalsSelf.Play("Close_MedalSelf", 0, 1);
            weeklyMedalsSelf.Play("Open_MedalSelf", 0, 1);
        }
        Clicked = false;
    }

    public void DailyMedalSelect()
    {
        if (medalIndex == 1)
        {
            StartCoroutine(AnimateSelf(weeklyMedalsSelf, dailyMedalsSelf, 0));
        }
        dailyMedalsImage.color = activeColor;
        weeklyMedalsImage.color = nonActiveColor;
    }

    public void WeeklyMedalSelect()
    {
        if (medalIndex == 0)
        {
            StartCoroutine(AnimateSelf(dailyMedalsSelf, weeklyMedalsSelf, 1));
        }
        dailyMedalsImage.color = nonActiveColor;
        weeklyMedalsImage.color = activeColor;
    }

    IEnumerator AnimateSelf(Animator closeSelf, Animator openSelf,int index)
    {
        if (!Clicked)
        {
            Clicked = true;
            medalIndex = index;
            if (closeSelf.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                closeSelf.Play("Close_MedalSelf", 0, 0);
                while (closeSelf.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
                {
                    yield return null;
                }
                yield return delay;
                openSelf.Play("Open_MedalSelf", 0, 0);
                Clicked = false;
            }
        }
    }
}
