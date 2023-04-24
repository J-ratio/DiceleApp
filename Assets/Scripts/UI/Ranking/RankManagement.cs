using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;

public class RankManagement : MonoBehaviour
{
    [SerializeField] Transform DailyBtn;
    [SerializeField] Transform WeeklyBtn;
    [SerializeField] Transform AllTimeBtn;


    [SerializeField] TextMeshProUGUI leaderboard_head_Text;

    [SerializeField] TextMeshProUGUI[] TopNames;
    [SerializeField] TextMeshProUGUI[] TopScores;


    TextMeshProUGUI[] RankedNames = new TextMeshProUGUI[10];
    TextMeshProUGUI[] RankedScores = new TextMeshProUGUI[10];


    [SerializeField] TextMeshProUGUI UserName;
    [SerializeField] TextMeshProUGUI UserScore;
    [SerializeField] TextMeshProUGUI UserRank;

    [SerializeField]  GameObject PlayerRank;
    [SerializeField] DataManager DataManager;

    

    // Start is called before the first frame update
    void Start()
    {
        GameObject Rank;
        for(int i = 0; i < 10; i++)
        {
            Rank = Instantiate(PlayerRank, transform);
            Rank.transform.GetChild(3).transform.GetComponent<TextMeshProUGUI>().text = (i+1).ToString();
            RankedNames[i] = Rank.transform.GetChild(7).transform.GetComponent<TextMeshProUGUI>();
            RankedScores[i] = Rank.transform.GetChild(6).transform.GetComponent<TextMeshProUGUI>();
        }
        UserName.text = DataManager.userName;

        OnPress(0);
    }


    public void OnPress(int x)
    {
        if(x == 0)
        {
            ShowDailyBoard();
            DailyBtn.GetChild(0).gameObject.SetActive(false);
            DailyBtn.GetChild(1).gameObject.SetActive(true);
            WeeklyBtn.GetChild(0).gameObject.SetActive(true);
            WeeklyBtn.GetChild(1).gameObject.SetActive(false);
            AllTimeBtn.GetChild(0).gameObject.SetActive(true);
            AllTimeBtn.GetChild(1).gameObject.SetActive(false);
        }
        else if(x == 1)
        {
            ShowWeeklyBoard();
            DailyBtn.GetChild(0).gameObject.SetActive(true);
            DailyBtn.GetChild(1).gameObject.SetActive(false);
            WeeklyBtn.GetChild(0).gameObject.SetActive(false);
            WeeklyBtn.GetChild(1).gameObject.SetActive(true);
            AllTimeBtn.GetChild(0).gameObject.SetActive(true);
            AllTimeBtn.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            ShowAllTimeBoard();
            DailyBtn.GetChild(0).gameObject.SetActive(true);
            DailyBtn.GetChild(1).gameObject.SetActive(false);
            WeeklyBtn.GetChild(0).gameObject.SetActive(true);
            WeeklyBtn.GetChild(1).gameObject.SetActive(false);
            AllTimeBtn.GetChild(0).gameObject.SetActive(false);
            AllTimeBtn.GetChild(1).gameObject.SetActive(true);
        }
    }



    void ShowDailyBoard()
    {
        List<DataManager.userStats> userStatsList = DataManager.DailyRankList;
        List<DataManager.userStats> topUserStatsList = DataManager.DailyTopRankList;



        for(int i = 0; i < 10; i++)
        {
            if( i < userStatsList.Count() )
            {
                RankedNames[i].text = userStatsList[i].userName;
                RankedScores[i].text = userStatsList[i].score.ToString();
            }
            else
            {
                RankedNames[i].text = "--";
                RankedScores[i].text = "--";
            }
        }

        for(int i = 0; i < 3; i++)
        {
            if( i < topUserStatsList.Count() )
            {
                TopNames[i].text = topUserStatsList[i].userName;
                TopScores[i].text = topUserStatsList[i].score.ToString();
            }
            else
            {
                TopNames[i].text = "--";
                TopScores[i].text = "--";
            }
        }

        UserScore.text = DataManager.PlayerDailyData.score.ToString();
        UserRank.text = DataManager.PlayerDailyData.userRank.ToString();
        
    }


    void ShowWeeklyBoard()
    {

        List<DataManager.userStats> userStatsList = DataManager.WeeklyRankList;
        List<DataManager.userStats> topUserStatsList = userStatsList;



        for(int i = 0; i < 10; i++)
        {
            if( i < userStatsList.Count() )
            {
                RankedNames[i].text = userStatsList[i].userName;
                RankedScores[i].text = userStatsList[i].score.ToString();
            }
            else
            {
                RankedNames[i].text = "--";
                RankedScores[i].text = "--";
            }
        }

        for(int i = 0; i < 3; i++)
        {
            if( i < topUserStatsList.Count() )
            {
                TopNames[i].text = topUserStatsList[i].userName;
                TopScores[i].text = topUserStatsList[i].score.ToString();
            }
            else
            {
                TopNames[i].text = "--";
                TopScores[i].text = "--";
            }
        }

        UserScore.text = DataManager.PlayerWeeklyData.score.ToString();
        UserRank.text = DataManager.PlayerWeeklyData.userRank.ToString();
        

    }


    async void ShowAllTimeBoard()
    {
        
        List<DataManager.userStats> userStatsList = DataManager.AllTimeRankList;
        List<DataManager.userStats> topUserStatsList = userStatsList;



        for(int i = 0; i < 10; i++)
        {
            if( i < userStatsList.Count() )
            {
                RankedNames[i].text = userStatsList[i].userName;
                RankedScores[i].text = userStatsList[i].allTimeScore.ToString();
            }
            else
            {
                RankedNames[i].text = "--";
                RankedScores[i].text = "--";
            }
        }

        for(int i = 0; i < 3; i++)
        {
            if( i < topUserStatsList.Count() )
            {
                TopNames[i].text = topUserStatsList[i].userName;
                TopScores[i].text = topUserStatsList[i].allTimeScore.ToString();
            }
            else
            {
                TopNames[i].text = "--";
                TopScores[i].text = "--";
            }
        }

        UserScore.text = DataManager.PlayerAllTimeData.allTimeScore.ToString();
        UserRank.text = DataManager.PlayerAllTimeData.userRank.ToString();

    }
}
