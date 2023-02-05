using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PST : MonoBehaviour
{
    private bool clicked;

    [SerializeField] private float speed;

    [SerializeField] private Animator medalsAnime;
    [SerializeField] private Animator statsAnime;
    [SerializeField] private Animator trophyAnime;

    [SerializeField] private PSTBase medalsLayOut;
    [SerializeField] private PSTBase statsLayOut;
    [SerializeField] private PSTBase trophyLayOut;

    private int pstIndex;//0=>medals,1=>stats,2=>trophy

    // Start is called before the first frame update
    void OnEnable()
    {
        if (pstIndex == 0)
        {
            medalsAnime.SetLayerWeight(1, 1);
        }
        else if (pstIndex == 1)
        {
            statsAnime.SetLayerWeight(1, 1);
        }
        else
        {
            trophyAnime.SetLayerWeight(1, 1);
        }
        EnableLayout();
    }
    void ResetLayerWeight()
    {
        if (pstIndex != 0) medalsAnime.SetLayerWeight(1, 0);
        if (pstIndex != 1) statsAnime.SetLayerWeight(1, 0);
        if (pstIndex != 2) trophyAnime.SetLayerWeight(1, 0);
    }
    bool CheckLayoutInteractions()
    {
        if (medalsLayOut.Clicked) return false;

        return true;
    }

    void EnableLayout()
    {
        medalsLayOut.gameObject.SetActive(false);
        statsLayOut.gameObject.SetActive(false);
        trophyLayOut.gameObject.SetActive(false);
        if (pstIndex == 0)
        {
            medalsLayOut.gameObject.SetActive(true);
        }
        else if (pstIndex == 1)
        {
            statsLayOut.gameObject.SetActive(true);
        }
        else
        {
            trophyLayOut.gameObject.SetActive(true);
        }
    }
    public void SelectMedals()
    {
        if(CheckLayoutInteractions()) StartCoroutine(Selected(medalsAnime, 0));
    }
    public void SelectStats()
    {
        if (CheckLayoutInteractions()) StartCoroutine(Selected(statsAnime, 1));
    }
    public void SelectTrophy()
    {
        if (CheckLayoutInteractions()) StartCoroutine(Selected(trophyAnime, 2));
    }

    IEnumerator Selected(Animator anime, int index)
    {
        if (!clicked)
        {
            clicked = true;
            pstIndex = index;
            ResetLayerWeight();
            EnableLayout();
            while (anime.GetLayerWeight(1) < 1)
            {
                anime.SetLayerWeight(1, Mathf.Lerp(anime.GetLayerWeight(1), 1, Time.deltaTime * speed));
                if (anime.GetLayerWeight(1) > 0.9f)
                {
                    anime.SetLayerWeight(1, 1);
                }
                yield return null;
            }
            clicked = false;
        }
    }
}
