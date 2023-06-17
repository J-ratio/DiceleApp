using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MedalsManager : MonoBehaviour
{
    
    [SerializeField] GameObject[] DailyMedals;
    [SerializeField] GameObject[] WeeklyMedals;


    [SerializeField] Sprite[] Medals;

    [SerializeField] TextMeshProUGUI RankText;
    [SerializeField] TextMeshProUGUI Message;
    [SerializeField] Image MedalImage;

    DataManager DataManager;
    string medalShareMessage = "";



    void Awake()
    {
        DataManager = GetComponent<DataManager>();

        for(int i = 0; i < 5; i++)
        {
            DailyMedals[i].transform.GetChild(1).gameObject.SetActive(true);
            DailyMedals[i].transform.GetChild(2).gameObject.SetActive(false);
            WeeklyMedals[i].transform.GetChild(1).gameObject.SetActive(true);
            WeeklyMedals[i].transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void SetDailyRank()
    {
        int[] count = DataManager.medalTally;

        for(int i = 0; i < 5; i ++)
        {
            if(count[i] > 0)
            {
                DailyMedals[i].transform.GetChild(2).gameObject.SetActive(true);
                DailyMedals[i].transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = count[i].ToString();
            }
        }

        /*int temp;

        if(rank == 1 || rank == 2 || rank == 3)
        {
            temp = rank-1;
        }
        else if(rank < 11 && rank > 3)
        {
            temp = 3;
        }
        else if( rank < 101)
        {
            temp = 4;
        }
        else
        {
            temp = 5;
        }


        for(int i = 0; i < 5; i ++)
        {
            if(i >= temp)
            {
                DailyMedals[i].transform.GetChild(2).gameObject.SetActive(true);
            }
        }*/

    }



    public void SetWeeklyRank()
    {
        int rank = DataManager.PlayerWeeklyData.userRank;


        int temp;

        if(rank == 1 || rank == 2 || rank == 3)
        {
            temp = rank-1;
        }
        else if(rank < 11 && rank > 3)
        {
            temp = 3;
        }
        else if( rank < 101)
        {
            temp = 4;
        }
        else
        {
            temp = 5;
        }


        for(int i = 0; i < 5; i ++)
        {
            if(i >= temp)
            {
                WeeklyMedals[i].transform.GetChild(2).gameObject.SetActive(true);
            }
        }

    }


    public void ShowResults(bool isDaily){

        int rank = 0;
        if(isDaily){
            rank = DataManager.PlayerYesterdayData.userRank;
            RankText.text = "Rank " + rank.ToString();
            Message.text = "You topped last yesterdays leaderboard \nand won the medal";
        }
        else{
            rank = DataManager.PlayerWeeklyData.userRank;
            RankText.text = "Rank " + rank.ToString();
            Message.text = "You topped last week leaderboard \nand won the medal";
        }

    
        int temp;

        if(rank == 1 || rank == 2 || rank == 3)
        {
            temp = rank-1;
        }
        else if(rank < 11 && rank > 3)
        {
            temp = 3;
        }
        else if( rank < 101 && rank > 10)
        {
            temp = 4;
        }
        else
        {
            temp = 5;
        }


        if(temp == 5)
        {
            transform.GetComponent<UIManager>().OpenScreen("WeeklyNoMedal");
            medalShareMessage = "I ranked " + rank + " globally on Dicele\n#DiceleApp";
        }
        else {
            MedalImage.sprite = Medals[temp];
            transform.GetComponent<UIManager>().OpenScreen("WeeklyCongratulations");
            medalShareMessage = "🏅 I have topped the leaderboard on Dicele App and got " + (temp == 0? "1st": temp == 1? "2nd": temp == 2? "3rd": temp == 3? "Top 10": temp == 4? "Top 100" : "") + " Medal \n#DiceleApp";
        }

    }

    public void ShareMedalResult()
    {
        DataManager.shareMessage = medalShareMessage;
        DataManager.ShareText();
    }
}
