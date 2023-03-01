using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GamePlayTutorial : MonoBehaviour
{
    [SerializeField] private UnityEvent OnTutorialFinished;

    [SerializeField] private TutorialSO[] tutorialSoArray;

    [SerializeField] private GameObject canvasObj;

    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Image messageIcon;

    [SerializeField] private GameObject nextButton;

    public bool isClassic;

    [SerializeField] private int highlightSortOrder = 10;

    [SerializeField] private int normalSortOrder = 4;

    private List<Dice> dices = new List<Dice>();

    private GamePlaySwapping playParticle;

    private TutorialSO tutorialSo;

    private List<Transform> diceParent = new List<Transform>();

    [SerializeField] private HandAnimation handAnimation;

    [SerializeField] private DataManager dataManager;

    [SerializeField] private GameObject[] noTutorialObjects;

    [SerializeField] private Canvas sumList;

    private void Awake() => playParticle = GetComponent<GamePlaySwapping>();

    public bool Init(List<Dice> l_dices)
    {
        nextButton.SetActive(false);

        canvasObj.SetActive(false);

        if (!isClassic) return false;

        tutorialSo = null;

        for (int i = 0; i < tutorialSoArray.Length; i++)
        {
            if (tutorialSoArray[i].level == dataManager.ClassicLvlIndex + 1)
            {
                tutorialSo = tutorialSoArray[i];
                break;
            }
        }

        if (tutorialSo == null) return false;

        if (tutorialSo.tutorialTypes.Length == 0) return false;

        dices = l_dices;

        diceParent.Clear();

        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].isTutorial = true;
            diceParent.Add(dices[i].transform.parent);
        }

        StartCoroutine(StartTutorial());

        return true;
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitForEndOfFrame();
        NoTutorialObjects(false);
        TextMeshProUGUI[] texts = new TextMeshProUGUI[0];
        Color normalSumColor = Color.white;

        texts = sumList.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            normalSumColor = texts[0].color;
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = Color.white;
            }
        }
        sumList.sortingOrder = 10;

        if (tutorialSo != null)
        {
            if (tutorialSo.tutorialStart)
            {
                TutorialFinish();

                yield return new WaitForSeconds(3);
            }
        }
        for (int i = 0; i < tutorialSo.tutorialTypes.Length; i++)
        {
            if(!tutorialSo.tutorialStart)
                canvasObj.SetActive(false);

            yield return StartCoroutine(TutorialDone(tutorialSo.tutorialTypes[i].dice_1, tutorialSo.tutorialTypes[i].dice_2, tutorialSo.tutorialTypes[i]));
        }

        NoTutorialObjects(true);
        if (texts.Length > 0)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].color = normalSumColor;
            }
            sumList.sortingOrder = normalSortOrder;
        }
        canvasObj.SetActive(false);
    }

    private void NoTutorialObjects(bool active)
    {
        for (int i = 0; i < noTutorialObjects.Length; i++)
        {
            noTutorialObjects[i].SetActive(active);
        }
    }

    IEnumerator TutorialDone(int index_1, int index_2, TutorialSwapType tutorialSwapType)
    {
        Dice dice_1 = diceParent[index_1 - 1].GetChild(0).GetComponent<Dice>();
        Dice dice_2 = diceParent[index_2 - 1].GetChild(0).GetComponent<Dice>();

        handAnimation?.Init(dice_1,dice_2);
        messageIcon.sprite = tutorialSwapType.icon;
        messageText.text = string.Format(tutorialSwapType.message, dice_1.diceNumber, dice_2.diceNumber);
        dice_1.isTutorial = false;
        dice_2.isTutorial = false;
        dice_1.spriteRenderer.sortingOrder = highlightSortOrder;
        dice_2.spriteRenderer.sortingOrder = highlightSortOrder;
        canvasObj.SetActive(true);
        Transform parent = dice_2.transform.parent;
        while (playParticle.isSwapping || dice_1.transform.parent != parent)
        {
            if (dice_1.transform.parent == parent)
                handAnimation?.Hide();
            yield return null;
        }

        dice_1.spriteRenderer.sortingOrder = normalSortOrder;
        dice_2.spriteRenderer.sortingOrder = normalSortOrder;
    }
    public void TutorialFinish()
    {
        if (tutorialSo == null) return;
        handAnimation?.Hide();
        canvasObj.SetActive(true);
        messageIcon.sprite = tutorialSo.tutorialFinishType.icon;
        messageText.text = string.Format(tutorialSo.tutorialFinishType.message);
        //nextButton.SetActive(true);
    }

    public void CloseTutorial()
    {
        OnTutorialFinished?.Invoke();
        for (int i = 0; i < dices.Count; i++)
        {
            dices[i].isTutorial = false;
        }
        canvasObj.SetActive(false);
        nextButton.SetActive(false);
    }
}
