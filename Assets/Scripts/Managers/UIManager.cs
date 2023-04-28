using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] screens;

    //List of UI elements stored in ResultScreenUI
    //0 => ScoreText Center, 1 => ScoreText Left, 2 => RankText, 3 => XPText, 4 => CoinsText
    [SerializeField] private TextMeshProUGUI[] ResultScreenUI;

    //0 => ScoreHead Center, 1 => ScoreHead Left, 2 => RankHead
    public GameObject[] ResultScreenScoreUI;


    //List of UI elements stored in MainScreenUI
    //0 => XpText, 1 => CoinsText, 2 => MonthText, 3 => DayText, 4 => ClassicLevelText, 5 => ProfileXpText
    [SerializeField] private TextMeshProUGUI[] MainScreenUI;

    [SerializeField] private TextMeshProUGUI title;

    [SerializeField] private int index = 0;


    //List of UI elements stored in GameScreenUI
    //0 => CoinText, 1 => DayText, 2 => MonthText, 3 => UndoChargeText, 4 => HintChargeText
    [SerializeField] public TextMeshProUGUI[] GameScreenUI;

    //List of UI elements stored in CalanderScreenUI
    //0 => XpText, 1 => CoinText
    [SerializeField] private TextMeshProUGUI[] CalanderScreenUI;


    //List of UI elements stored in Stats Screen
    //0 => GamesPlayed, 1 => BestRank, 2 => CurrentStreak, 3 => BestStreak, 4 => TotalStars, 5 => Win%, 6 => Average Score, 7 => Best Score
    [SerializeField] private TextMeshProUGUI[] StatsScreenUI;

    //PiggyScreenTexts
    //0 => winAwaytext, 1 =>yetToWinText , 2  => winGoldText, 3 => lostWinsWayText, 4 => LoseGoldText
    [SerializeField]  private TextMeshProUGUI[] PiggyScreenUI;

    [SerializeField] TextMeshProUGUI[] UserNameText;
    [SerializeField] public TMP_InputField UserNameInputText;

    [SerializeField] GameObject[] Avatars;
    [SerializeField] Image Avatar;

    void OnEnable()
    {
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        MainScreenUI[2].text = DateTime.Now.ToString("MMMM").Substring(0, 3);
        MainScreenUI[3].text = DateTime.Now.Day.ToString();


        title.text = screens[index].name;

        screens[index].SetActive(true);
    }
    public void Next()
    {
        screens[index].SetActive(false);

        index++;

        if (index > screens.Length - 1) index = 0;

        title.text = screens[index].name;

        screens[index].SetActive(true);
    }
    public void Previous()
    {
        screens[index].SetActive(false);

        index--;

        if (index < 0) index = screens.Length - 1;

        title.text = screens[index].name;

        screens[index].SetActive(true);
    }


    public void OpenScreen(string name)
    {
        foreach (GameObject screen in screens)
        {
            //Debug.Log(screen.name);
            if (screen.name == name)
            {
                screen.SetActive(true);
                break;
            }
        }
    }

    public void CloseScreen(string name)
    {
        foreach (GameObject screen in screens)
        {
            if (screen.name == name)
            {
                screen.SetActive(false);
                break;
            }
        }
    }



    //ResultScreenUpdateFuntions

    public void UpdateResultScreen(int score, int rank, int Xp, int Coins, bool showRank)
    {
        ResultScreenUI[0].text = score.ToString();
        ResultScreenUI[1].text = score.ToString();
        ResultScreenUI[2].text = rank.ToString();
        ResultScreenUI[3].text = Xp.ToString();
        ResultScreenUI[4].text = Coins.ToString();


        if (showRank)
        {
            ResultScreenScoreUI[0].SetActive(false);
            ResultScreenScoreUI[1].SetActive(true);
            ResultScreenScoreUI[2].SetActive(true);
        }
        else
        {
            ResultScreenScoreUI[0].SetActive(true);
            ResultScreenScoreUI[1].SetActive(false);
            ResultScreenScoreUI[2].SetActive(false);
        }
    }


    //MainScreenUpdateFuntions

    public void UpdatePlayerStats(int xp, int coins)
    {
        MainScreenUI[0].text = xp.ToString();
        MainScreenUI[5].text = xp.ToString();
        MainScreenUI[1].text = coins.ToString();
        CalanderScreenUI[0].text = xp.ToString();
        CalanderScreenUI[1].text = coins.ToString();
        GameScreenUI[0].text = coins.ToString();
    }


    public void UpdateClassicLevel(int lvl)
    {
        MainScreenUI[4].text = "LEVEL " + lvl.ToString();
    }


    //GameScreenUpdateFunctions

    public void UpdateGameScreen(int PlayerCoins, int DayNum, string MonthStr, int HintCharge, int UndoCharge)
    {
        GameScreenUI[0].text = PlayerCoins.ToString();
        GameScreenUI[1].text = DayNum.ToString();
        GameScreenUI[2].text = MonthStr;
        GameScreenUI[3].text = UndoCharge.ToString();
        GameScreenUI[4].text = HintCharge.ToString();
    }


    public void UpdateStatsScreenUI(int[] StatsArr)
    {
        for(int i = 0; i < StatsArr.Count(); i++)
        {
            StatsScreenUI[i].text = StatsArr[i].ToString(); 
        }
    }


    public void UpdateName(string name)
    {
        UserNameText[0].text  = name;
        UserNameText[1].text  = name;
        UserNameInputText.text = name;
    }


    public void UpdateAvatar(int num)
    {
        Avatar.sprite = Avatars[num].GetComponent<Image>().sprite;

        for(int i = 0; i < Avatars.Count(); i++)
        {
            Avatars[i].transform.GetChild(0).gameObject.SetActive(false);
        }

        Avatars[num].transform.GetChild(0).gameObject.SetActive(true);
    }


    public void UpdatePiggyScreen(int Gold, int winsAway, int action)
    {
        if(action == 0)
        {
            if(winsAway != 1) PiggyScreenUI[0].text = winsAway.ToString() + " Wins to unlock,win and \ncontinue filling";
            else PiggyScreenUI[0].text = winsAway.ToString() + " Win to unlock,win and \ncontinue filling";
            
            PiggyScreenUI[1].text = Gold.ToString();

        }
        else if(action == 1)
        {
            PiggyScreenUI[2].text = Gold.ToString();
        }
        else
        {
            if(winsAway != 1) PiggyScreenUI[3].text = "You are just " + winsAway.ToString() + " wins away to unlock" + "\nthe piggy bank" ;
            else PiggyScreenUI[3].text = "You are just " + winsAway.ToString() + " win away to unlock" + "\nthe piggy bank" ;


            PiggyScreenUI[4].text = Gold.ToString();
        }
    }


    public void PolicyRedirect()
    {
        Application.OpenURL("https://dicele.com/privacy-policy.html");
    }

    public void EmailRedirect()
    {
        Application.OpenURL("mailto:contact@sagacistudios.com");
    }

    void OnDisable()
    {
    }

}