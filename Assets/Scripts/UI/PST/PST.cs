using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private int selectHash = Animator.StringToHash("SelectingPST");
    private int deSelectHash = Animator.StringToHash("DeSelectingPST");
    [SerializeField] private ScrollRect rect;
    private float targetNormalizePosition;

    private void Awake()
    {
        ChangeWidth(medalsLayOut.GetComponent<RectTransform>());
        ChangeWidth(statsLayOut.GetComponent<RectTransform>());
        ChangeWidth(trophyLayOut.GetComponent<RectTransform>());
        ResetLayerWeight();
        SelectButton();
    }

    void ChangeWidth(RectTransform rectTransform)
    {
        rectTransform.sizeDelta = new Vector2(Screen.width, rectTransform.sizeDelta.y);
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        ResetLayerWeight();
        SelectButton();
        //EnableLayout();
    }

    private void SelectButton()
    {
        if (pstIndex == 0)
        {
            medalsAnime.Play(selectHash, 0, 0);
            targetNormalizePosition = 0;
            StartCoroutine(MoveRectPosition());
        }
        else if (pstIndex == 1)
        {
            statsAnime.Play(selectHash, 0, 0);
            targetNormalizePosition = 0.5f;
            StartCoroutine(MoveRectPosition());
        }
        else
        {
            trophyAnime.Play(selectHash, 0, 0);
            targetNormalizePosition = 1;
            StartCoroutine(MoveRectPosition());
        }
    }

    IEnumerator MoveRectPosition()
    {
        while (targetNormalizePosition != rect.horizontalNormalizedPosition)
        {
            rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targetNormalizePosition, Time.deltaTime * speed);
            yield return null;
        }
    }

    void ResetLayerWeight()
    {
        if (pstIndex != 0) medalsAnime.Play(deSelectHash, 0, 1);
        if (pstIndex != 1) statsAnime.Play(deSelectHash, 0, 1);
        if (pstIndex != 2) trophyAnime.Play(deSelectHash, 0, 1);
    }
    bool CheckLayoutInteractions()
    {
        if (medalsLayOut.Clicked) return false;

        return true;
    }

    public void SelectMedals()
    {
        if (CheckLayoutInteractions()) StartCoroutine(Selected(medalsAnime, 0));
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
            SelectButton();
            yield return new WaitForSeconds(0.1f);
            clicked = false;
        }
    }
}
