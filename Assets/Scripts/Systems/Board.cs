using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class Board : MonoBehaviour
{
    [SerializeField]
    private GameObject dicePrefab;
    [SerializeField]
    private GameObject diceSumBorder;
    [SerializeField]
    private GameObject VertSumList;
    [SerializeField]
    private GameObject HorSumList;
    [SerializeField]
    private GameObject sumImg;
    [SerializeField]
    private TextMeshProUGUI movesText;
    [SerializeField]
    GameObject UndoButton;
    [SerializeField]
    GameObject HintButton;

    [SerializeField]
    private HandAnimation hand;



    public int Num;
    public int GreenMinBound;
    int matched;
    int swapped;
    int time;


    int currentUndoNum;
    int currentHintNum;



    [SerializeField]
    private Sprite[] whiteDiceImages;
    [SerializeField]
    private Sprite[] yellowDiceImages;
    [SerializeField]
    private Sprite[] greenDiceImages;
    [SerializeField]
    private TextMeshProUGUI[] SumText;


    public List<List<int>> solArray = new List<List<int>>();
    public List<List<int>> spawnArray = new List<List<int>>();
    public List<List<int>> lineSolutionList = new List<List<int>>();
    public List<int> emptySlots = new List<int>();


    int[] freqList = { -1, -1, -1, -1, -1, -1 };
    int[] freqListDuplicate = new int[6];

    [SerializeField]
    private List<Dice> diceList;
    private List<Dice> previousSwappedDice = new List<Dice>();

    //Added by charan
    [SerializeField] private GamePlayAnimation playAnimation;
    [SerializeField] private GamePlaySwapping gamePlayParticle;
    [SerializeField] private GamePlayTutorial gamePlayTutorial;
    public bool isTutorial;
    //...


    public AudioSource winSound;
    public AudioSource loseSound;
    public AudioSource swapSound;
    public AudioSource undoSound;
    public AudioSource gameWinSound;
    public AudioSource matchSound;
    public AudioSource greenDiceSound;


    void OnEnable()
    {
        ActionEvents.swapDice += SwapDice;
        ActionEvents.SendGameData += ReceiveGameData;
        ActionEvents.Undo += Undo;
        ActionEvents.Hint += Hint;
    }

    void Start()
    {
        //ReceiveGameData(new List<int>() {5, 5, 2, 3, 1, 3, 6, 0, 6, 4, 4, 4, 3, 5, 0, 3, 6, 0, 6, 0, 1, 2, 1, 1, 2}, new List<int>() {0, 2, 4, 1, 5, 2, 6, 5, 6, 0, 3, 3, 5, 0, 2, 0, 6, 1, 6, 1, 3, 1, 3, 4, 4}, 0);
        //StartGame();
    }

    void StartGame()
    {

        SetSize();
        //createDiceFreq();
        //generateSolutionArray();
        //generateSpawnArray();
        CreateLineSolutionList();
        CreateDice();
        StartCoroutine("Timer");
        movesText.text = (Num*Num - emptySlots.Count - swapped).ToString();
        currentUndoNum = 0;
        currentHintNum = 0;

        UndoButton.GetComponent<Button>().enabled = false;
        UndoButton.transform.GetChild(0).gameObject.SetActive(false);
        UndoButton.transform.GetChild(1).gameObject.SetActive(true);

        HintButton.GetComponent<Button>().enabled = true;

    }

    private void createDiceFreq()
    {

        List<int> diceNumOptions = new List<int>() { 0, 1, 2, 3, 4, 5 };

        for (int i = 0; i < (Num * Num - emptySlots.Count) % 6; i++)
        {
            int randIndex = Random.Range(0, diceNumOptions.Count);
            int random = diceNumOptions[randIndex];
            freqList[random] = (Num * Num - emptySlots.Count) / 6 + 1;
            diceNumOptions.Remove(random);
        }

        for (int i = 0; i < 6; i++)
        {
            if (freqList[i] == -1)
            {
                freqList[i] = (Num * Num - emptySlots.Count) / 6;
            }
            freqListDuplicate[i] = freqList[i];
        }
        //LogFreqList();

    }


    private void generateSolutionArray()
    {

        for (int i = 0; i < Num; i++)
        {
            solArray.Add(new List<int>());
        }

        for (var k = 0; k < Num; k++)
        {
            for (var j = 0; j < Num; j++)
            {
                if (!emptySlots.Contains(k * Num + j + 1))
                {
                    int rnd = Random.Range(0, 6);
                    int t = 0;
                    while (freqListDuplicate[rnd] <= 0)
                    {
                        rnd = (rnd + 1) % 6;
                        t++;
                        if (t == 7)
                            break;
                    }

                    solArray[k].Add(rnd);
                    freqListDuplicate[rnd]--;
                }
                else
                {
                    solArray[k].Add(-1);
                }
            }
        }
    }


    private void CreateLineSolutionList()
    {
        List<int> SumList = new List<int>();

        for (int i = 0; i < 2 * Num; i++)
        {
            lineSolutionList.Add(new List<int>());
            SumList.Add(0);

            for (int j = 0; j < Num; j++)
            {
                if (i < Num)
                {
                    //if(!emptySlots.Contains(i + j*Num + 1))
                    //{
                    lineSolutionList[i].Add(solArray[j][i]);
                    if (solArray[j][i] != -1)
                    {
                        SumList[i] = SumList[i] + solArray[j][i] + 1;
                    }
                    //}
                }
                else
                {
                    //if(!emptySlots.Contains((i-Num)*Num + j + 1))
                    //{
                    lineSolutionList[i].Add(solArray[i - Num][j]);
                    if (solArray[i - Num][j] != -1)
                    {
                        SumList[i] = SumList[i] + solArray[i - Num][j] + 1;
                    }
                    //}
                }

                if (i == 1 || i == 3 || i == 6 || i == 8)
                {
                    //SumText[i].text = (SumList[i] - 14).ToString();
                }
                else
                {
                    //SumText[i].text = SumList[i].ToString();
                }

            }

        }

        CreateSum(SumList);


        //ActionEvents.UpdateSum(SumList,Num);
        //LogLineSol();
    }


    void LogLineSol()
    {
        for (int i = 0; i < 2 * Num; i++)
        {
            string result = "";
            foreach (var item in lineSolutionList[i])
            {
                result += item.ToString() + ", ";
            }
            Debug.Log(result);
        }
        Debug.Log("-----------------------------------------------");

    }


    private void generateSpawnArray()
    {
        int[] freqListClone = new int[6];
        for (int i = 0; i < 6; i++)
        {
            freqListClone[i] = freqList[i];
        }

        List<int> MatchedSlot = new List<int>(GreenMinBound);
        for (int i = 0; i < GreenMinBound; i++)
        {
            int rnd = Random.Range(0, Num * Num);
            int t = 0;
            while (MatchedSlot.Contains(rnd) || emptySlots.Contains(rnd + 1) || freqListClone[solArray[rnd / Num][rnd % Num]] <= 0)
            {
                rnd = (rnd + 1) % (Num * Num);
                t++;
                if (t == Num * Num)
                    break;
            }
            MatchedSlot.Add(rnd);
            freqListClone[solArray[rnd / Num][rnd % Num]]--;
            LogFreqList();
        }


        for (int i = 0; i < Num; i++)
        {
            spawnArray.Add(new List<int>());
        }

        LogFreqList();

        for (var k = 0; k < Num; k++)
        {
            for (var j = 0; j < Num; j++)
            {
                if (!emptySlots.Contains(k * Num + j + 1))
                {
                    if (!(MatchedSlot.Contains(k * Num + j)))
                    {
                        int rnd = Random.Range(0, 6);
                        int t = 0;
                        while (freqListClone[rnd] <= 0 || rnd == solArray[k][j])
                        {
                            rnd = (rnd + 1) % 6;
                            t++;
                            if (t == 7)
                                break;
                        }

                        spawnArray[k].Add(rnd);
                        freqListClone[rnd]--;
                        LogFreqList();
                    }
                    else
                    {
                        spawnArray[k].Add(solArray[k][j]);
                    }

                }
                else
                {
                    spawnArray[k].Add(-1);
                }
            }
        }
        if (freqListClone.Count(x => x < 0) > 0)
        {
            spawnArray.Clear();
            MatchedSlot.Clear();
            generateSpawnArray();
        }



        void LogFreqList()
        {
            //string s1 = "";
            string s2 = "";
            for (int i = 0; i < 6; i++)
            {
                //s1 = s1 + freqList[i] + " ";
                s2 = s2 + freqListClone[i] + " ";
            }
            //Debug.Log(s1);
        }

    }


    private void CreateSum(List<int> SumList)
    {
        for (var i = 0; i < 2 * Num; i++)
        {
            GameObject sum;
            if (i < Num)
            {
                sum = Instantiate(diceSumBorder, transform);
                sum.transform.parent = HorSumList.transform;
            }
            else
            {
                sum = Instantiate(diceSumBorder, transform);
                sum.transform.parent = VertSumList.transform;
            }

            if (emptySlots.Contains(i + 1)) sum.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SumList[i].ToString();
            else sum.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = SumList[i].ToString();
        }
    }


    private void CreateDice()
    {
        diceList = new List<Dice>();

        for (var i = 0; i < Num * Num; i++)
        {
            Dice dice = Instantiate(dicePrefab, transform).transform.GetChild(0).GetComponent<Dice>();
            dice.slotPos = new Vector2Int(i / Num, i % Num);
            dice.slotsolution = solArray[i / Num][i % Num];
            dice.diceNumber = spawnArray[i / Num][i % Num];
            //Added by charan
            dice.Init(gamePlayParticle, this);
            //
            if (emptySlots.Contains(i + 1))
            {
                dice.GetComponentInParent<Image>().enabled = false;
                dice.spriteRenderer.enabled = false;
                dice.boxCollider.enabled = false;
            }
            else
            {
                if (dice.slotsolution == dice.diceNumber)
                {
                    dice.spriteRenderer.sprite = greenDiceImages[spawnArray[i / Num][i % Num]];
                    dice.colorType = ColorType.Green;
                    dice.boxCollider.enabled = false;
                    lineSolutionList[dice.slotPos[1]].Remove(dice.diceNumber);
                    lineSolutionList[dice.slotPos[0] + Num].Remove(dice.diceNumber);

                    if(lineSolutionList[dice.slotPos[1]].Count(x => x != -1) == 0)
                    {
                        HorSumList.transform.GetChild(dice.slotPos[1]).GetComponent<Animator>().enabled = true;
                    }

                    if(lineSolutionList[dice.slotPos[0]+Num].Count(x => x != -1) == 0)
                    {
                        VertSumList.transform.GetChild(dice.slotPos[0]).GetComponent<Animator>().enabled = true;
                    }
                    
                    matched++;
                    dice.matched = true;
                }
            }
            diceList.Add(dice);
            dice.gameObject.name = "dice_" + i;
        }

        playAnimation?.AddDice(diceList, Num);
        CheckColor();
        StartTutorial();
    }

    //Added by charan
    void StartTutorial()
    {
        isTutorial = gamePlayTutorial.Init(diceList);
    }
    //....

    public void ClearDice()
    {
        matched = 0;
        foreach (Dice dice in diceList)
        {
            Destroy(dice.transform.parent.gameObject);
        }

        for (int i = 0; i < Num; i++)
        {
            Destroy(HorSumList.transform.GetChild(i).gameObject);
            Destroy(VertSumList.transform.GetChild(i).gameObject);
        }

        diceList.Clear();

    }

    private void CheckColor()
    {
        foreach (Dice dice in diceList)
        {
            if (dice.slotsolution != dice.diceNumber)
            {
                if (lineSolutionList[dice.slotPos[1]].Contains(dice.diceNumber) || lineSolutionList[dice.slotPos[0] + Num].Contains(dice.diceNumber))
                {
                    dice.spriteRenderer.sprite = yellowDiceImages[dice.diceNumber];
                    dice.colorType = ColorType.Yellow;
                }
                else
                {
                    dice.spriteRenderer.sprite = whiteDiceImages[dice.diceNumber];
                    dice.colorType = ColorType.White;
                }
            }
        }
    }

    //Added by charan
    internal bool CheckSwapDice(Dice swap_dice)
    {
        Dice dice = FindClosestDice(swap_dice);
        if (dice.isTutorial)
            StartCoroutine(MoveTowardsTarget(swap_dice, swap_dice.GetInitialPos(), 10));
        return dice.isTutorial;
    }
    //....

    private void SwapDice(Dice swap_dice)
    {
        hand.gameObject.SetActive(false);

        Dice dice = FindClosestDice(swap_dice);
        previousSwappedDice.Clear();
        previousSwappedDice.Add(dice);
        previousSwappedDice.Add(swap_dice);
        if (dice == swap_dice)
        {
            StartCoroutine(MoveTowardsTarget(swap_dice, dice.GetInitialPos(), 10));
        }
        else
        {
            
            playSwapSound();

            Vector2 posTemp = swap_dice.initialPos;
            swap_dice.initialPos = dice.initialPos;
            dice.initialPos = posTemp;

            Vector2Int slotTemp = swap_dice.slotPos;
            swap_dice.slotPos = dice.slotPos;
            dice.slotPos = slotTemp;

            int solTemp = swap_dice.slotsolution;
            swap_dice.slotsolution = dice.slotsolution;
            dice.slotsolution = solTemp;


            if (dice.slotsolution == dice.diceNumber)
            {
                dice.spriteRenderer.sprite = greenDiceImages[dice.diceNumber];
                dice.colorType = ColorType.Green;
                dice.GetComponent<BoxCollider2D>().enabled = false;
                lineSolutionList[dice.slotPos[1]].Remove(dice.diceNumber);
                lineSolutionList[dice.slotPos[0] + Num].Remove(dice.diceNumber);
                dice.matched = true;
                matched++;

                if(lineSolutionList[dice.slotPos[1]].Count(x => x != -1) == 0)
                {
                    HorSumList.transform.GetChild(dice.slotPos[1]).GetComponent<Animator>().enabled = true;
                    if(matched < Num*Num - emptySlots.Count - 1) playMatchSound();
                }

                if(lineSolutionList[dice.slotPos[0]+Num].Count(x => x != -1) == 0)
                {
                    VertSumList.transform.GetChild(dice.slotPos[0]).GetComponent<Animator>().enabled = true;
                    if(matched < Num*Num - emptySlots.Count - 1) playMatchSound();
                }
            }
            if (swap_dice.slotsolution == swap_dice.diceNumber)
            {
                swap_dice.spriteRenderer.sprite = greenDiceImages[swap_dice.diceNumber];
                swap_dice.colorType = ColorType.Green;
                swap_dice.GetComponent<BoxCollider2D>().enabled = false;
                lineSolutionList[swap_dice.slotPos[1]].Remove(swap_dice.diceNumber);
                lineSolutionList[swap_dice.slotPos[0] + Num].Remove(swap_dice.diceNumber);

                matched++;
                swap_dice.matched = true;
                
                if(lineSolutionList[swap_dice.slotPos[1]].Count(x => x != -1) == 0)
                {
                    HorSumList.transform.GetChild(swap_dice.slotPos[1]).GetComponent<Animator>().enabled = true;
                    if(matched < Num*Num - emptySlots.Count - 1) playMatchSound();
                }
                
                if(lineSolutionList[swap_dice.slotPos[0]+Num].Count(x => x != -1) == 0)
                {
                    VertSumList.transform.GetChild(swap_dice.slotPos[0]).GetComponent<Animator>().enabled = true;
                    if(matched < Num*Num - emptySlots.Count - 1) playMatchSound();
                }
            }

            //LogLineSol();

            swapped++;

            if(currentUndoNum < 3) 
            {
                UndoButton.GetComponent<Button>().enabled = true;
                UndoButton.transform.GetChild(0).gameObject.SetActive(true);
                UndoButton.transform.GetChild(1).gameObject.SetActive(false);
            }

            if(currentHintNum < 4)
            {
                HintButton.GetComponent<Button>().enabled = true;
                HintButton.transform.GetChild(0).gameObject.SetActive(true);
                HintButton.transform.GetChild(1).gameObject.SetActive(false);
            }

            movesText.text = (Num*Num - emptySlots.Count - swapped).ToString();

            ActionEvents.UpdateGameState(dice.slotPos[0] * Num + dice.slotPos[1], swap_dice.slotPos[0] * Num + swap_dice.slotPos[1]);

            CheckColor();

            //Modified by charan
            if (isTutorial)
                gamePlayParticle.SwapDices(dice, swap_dice, SwapDone, 10, 4);
            else
                gamePlayParticle.SwapDices(dice, swap_dice, SwapDone, 5, 4);


            void SwapDone()
            {
                if (matched == Num * Num - emptySlots.Count)
                {
                    UndoButton.GetComponent<Button>().enabled = false;
                    HintButton.GetComponent<Button>().enabled = false;

                    playGameWinSound();

                    playAnimation.CheckAndPlayFinalHorizontalAnimation(VerticleAlso);
                    isTutorial = false;
                }
                else if (swapped == Num * Num - emptySlots.Count)
                {
                    GameLose();
                }
            }
            //.....
        }
    }



    //Added by charan
    void VerticleAlso() => playAnimation.CheckAndPlayFinalVerticalAnimation(GameWin);
    public void StopTutorial() => isTutorial = false;
    public void FinalTutorial()
    {
        isTutorial = false;
        gamePlayTutorial?.TutorialFinish();
    }
    //...


    public void GameWin()
    {
        playLevelWinSound();
        ClearDice();
        lineSolutionList.Clear();
        ActionEvents.TriggerGameWinEvent(swapped, time);
    }

    private void GameLose()
    {
        playLevelLoseSound();
        ClearDice();
        lineSolutionList.Clear();
        ActionEvents.TriggerGameLoseEvent();
    }


    private void ReceiveGameData(List<int> sol, List<int> spawn, int actionIndex, bool isDaily)
    {
        Num = (int)Mathf.Sqrt(sol.Count);

        swapped = 0;
        matched = 0;

        if (actionIndex == 1)   swapped = Num*Num - sol.Count(x => x == -1) - 3;

        solArray.Clear();
        spawnArray.Clear();
        emptySlots.Clear();


        for (int i = 0; i < Num; i++)
        {
            solArray.Add(new List<int>());
            spawnArray.Add(new List<int>());
        }

        for (int j = 0; j < Num; j++)
        {
            for (int k = 0; k < Num; k++)
            {
                solArray[j].Add(sol[Num * j + k]);
                spawnArray[j].Add(spawn[Num * j + k]);

                if (sol[Num * j + k] == -1)
                {
                    emptySlots.Add(Num * j + k + 1);
                }
            }
        }
        StartGame();
    }



    IEnumerator MoveTowardsTarget(Dice gameObject, Vector2 position, float speed)
    {
        Vector3 final_pos = new Vector3(position.x, position.y, gameObject.transform.position.z);
        // Keep moving the game object towards the target position until it reaches the target position.
        while (gameObject.transform.position != final_pos)
        {
            // Move the game object towards the target position at the specified speed.
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, final_pos, speed * Time.deltaTime);

            if (Vector3.Distance(gameObject.transform.position, final_pos) < 0.1f)
                gameObject.transform.position = final_pos;
            // Yield control back to the main thread for the next frame.
            yield return null;
        }
        yield return null;
    }




    private Dice FindClosestDice(Dice swap_dice)
    {

        Vector2 diceDist = new Vector2(swap_dice.transform.position.x, swap_dice.transform.position.y);
        float minDist = 100f;
        Dice new_dice = swap_dice;

        foreach (Dice dice in diceList)
        {
            if (dice.diceNumber != dice.slotsolution)
            {
                float dist = Vector2.Distance(dice.GetInitialPos(), diceDist);
                if (dist < minDist)
                {
                    minDist = dist;
                    new_dice = dice;
                }
            }
        }
        return new_dice;
    }


    private void UpdateText()
    {
        string s1 = "";
        string s2 = "";
        for (var i = 0; i < Num; i++)
        {
            for (var j = 0; j < Num; j++)
            {
                if (j < lineSolutionList[i + Num].Count)
                {
                    s1 = s1 + lineSolutionList[i + Num][j] + " ";
                }
                if (j < lineSolutionList[i].Count)
                {
                    s2 = s2 + lineSolutionList[i][j] + " ";
                }
            }
            s1 = s1 + "\n";
            s2 = s2 + "\n";
        }
    }


    private void SetSize()
    {
        Vector2 horizontalInitPos = new Vector2(-50, -380);
        Vector2 verticleInitPos = new Vector2(395,100);
        Vector2 sumInitPos = new Vector2(382,-370);
        GetComponent<RectTransform>().sizeDelta = new Vector2(Num * 150, Num * 150);
        HorSumList.GetComponent<RectTransform>().sizeDelta = new Vector2(Num * 150, 125);
        VertSumList.GetComponent<RectTransform>().sizeDelta = new Vector2(125, Num * 150);
        HorSumList.GetComponent<RectTransform>().anchoredPosition = new Vector2(horizontalInitPos.x, horizontalInitPos.y + 80*(5 - Num));
        VertSumList.GetComponent<RectTransform>().anchoredPosition = new Vector2(verticleInitPos.x - 80*(5 - Num), verticleInitPos.y );
        sumImg.GetComponent<RectTransform>().anchoredPosition = new Vector2(sumInitPos.x - 80*(5 - Num), sumInitPos.y + 80*(5 - Num));

    }

    private void Undo()
    {

        currentUndoNum++;
        if(currentUndoNum > 4)
        {
            UndoButton.GetComponent<Button>().enabled = false;
            UndoButton.transform.GetChild(0).gameObject.SetActive(false);
            UndoButton.transform.GetChild(1).gameObject.SetActive(true);
            return;
        }

        playUndoSound();

        Dice swap_dice = previousSwappedDice[0];
        Dice dice = previousSwappedDice[1];


        if (dice.slotsolution == dice.diceNumber)
        {
            dice.GetComponent<BoxCollider2D>().enabled = true;
            lineSolutionList[dice.slotPos[1]].Add(dice.diceNumber);
            lineSolutionList[dice.slotPos[0] + Num].Add(dice.diceNumber);
            dice.matched = false;
            matched--;


        }
        if (swap_dice.slotsolution == swap_dice.diceNumber)
        {
            swap_dice.GetComponent<BoxCollider2D>().enabled = true;
            lineSolutionList[swap_dice.slotPos[1]].Add(swap_dice.diceNumber);
            lineSolutionList[swap_dice.slotPos[0] + Num].Add(swap_dice.diceNumber);
            matched--;
            swap_dice.matched = false;

        }

        Vector2 posTemp = swap_dice.initialPos;
        swap_dice.initialPos = dice.initialPos;
        dice.initialPos = posTemp;

        Vector2Int slotTemp = swap_dice.slotPos;
        swap_dice.slotPos = dice.slotPos;
        dice.slotPos = slotTemp;

        int solTemp = swap_dice.slotsolution;
        swap_dice.slotsolution = dice.slotsolution;
        dice.slotsolution = solTemp;

        swapped--;
        movesText.text = (Num*Num - emptySlots.Count - swapped).ToString();

        ActionEvents.UpdateGameState(dice.slotPos[0] * Num + dice.slotPos[1], swap_dice.slotPos[0] * Num + swap_dice.slotPos[1]);

        CheckColor();

        gamePlayParticle.SwapDices(dice,swap_dice,null,5,4);


        UndoButton.GetComponent<Button>().enabled = false;
        UndoButton.transform.GetChild(0).gameObject.SetActive(false);
        UndoButton.transform.GetChild(1).gameObject.SetActive(true);

    }


    private void Hint()
    {
        currentHintNum++ ;
        if(currentHintNum > 4)
        {
            HintButton.GetComponent<Button>().enabled = false;
            HintButton.transform.GetChild(0).gameObject.SetActive(false);
            HintButton.transform.GetChild(1).gameObject.SetActive(true);
            return;
        }

        Dice[] HintDice = makeHint();
        hand.waypoint_1 = HintDice[0].transform;
        hand.waypoint_2 = HintDice[1].transform;
        hand.ResetMovement();
        hand.StartAnimation();
        hand.gameObject.SetActive(true);

        HintButton.GetComponent<Button>().enabled = false;
        HintButton.transform.GetChild(0).gameObject.SetActive(false);
        HintButton.transform.GetChild(1).gameObject.SetActive(true);
    }


    Dice[] makeHint()
    {
        Dice[] hintDice = new Dice[2];
        foreach(Dice dice in diceList)
        {
            if(!dice.matched)
            {
                hintDice[0]= dice;
                break;
            }
        }

        foreach(Dice dice in diceList)
        {
            if(dice.slotsolution == hintDice[0].diceNumber && !dice.matched)
            {
                hintDice[1] = dice;
                break;
            }
        }

        return hintDice;
    }


    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            time++;
        }
    }



    void OnDisable()
    {
        ActionEvents.swapDice -= SwapDice;
        ActionEvents.SendGameData -= ReceiveGameData;
        ActionEvents.Undo -= Undo;
        ActionEvents.Hint -= Hint;
    }


    void playGameWinSound()
    {
        if(UIManager.SoundTogggle)
        {
            gameWinSound.Play();
            Invoke("playSecondtime", 1.0f*Num/4);
        }
        
    }

    void playSecondtime()
    {
        if(UIManager.SoundTogggle)
        gameWinSound.Play();
    }

    void playLevelLoseSound()
    {
        if(UIManager.SoundTogggle)
        loseSound.Play();
    }

    void playLevelWinSound()
    {
        if(UIManager.SoundTogggle)
        winSound.Play();
    }

    void playMatchSound()
    {
        if(UIManager.SoundTogggle)
        matchSound.Play();
    }

    void playSwapSound()
    {
        if(UIManager.SoundTogggle)
        swapSound.Play();
    }

    void playUndoSound()
    {
        if(UIManager.SoundTogggle)
        undoSound.Play();
    }

    void playGreenDiceSound()
    {
        if(UIManager.SoundTogggle)
        greenDiceSound.Play();
    }

}
