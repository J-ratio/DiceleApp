using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TrophyManager : MonoBehaviour
{

    [SerializeField] GameObject TrophyShelf;
    private DateTime StartDate = new DateTime(2023,1,10);
    private List<int> MonthlyStarsList = new List<int>();
    [SerializeField] DataManager DataManager;
    Transform[] TrophyList = new Transform[12];

    [SerializeField] Sprite[] ActiveTrophies;
    [SerializeField] Sprite[] DisabledTrophies;

    int[] trophyCount = new int[3];



    // Start is called before the first frame update
    void Awake()
    {
        GameObject t;
        for(int i = 0; i < 12; i++)
        {
            t = Instantiate(TrophyShelf, transform);
            TrophyList[i] = t.transform;
            TrophyList[i].GetChild(2).transform.GetComponent<TextMeshProUGUI>().text = GetMonthText(i);
            TrophyList[i].GetChild(5).transform.GetChild(3).transform.GetComponent<TextMeshProUGUI>().text = GetMonthStars(i).ToString();
            if(i < StartDate.Month - 1 || i > DateTime.Now.Month - 1) {
                TrophyList[i].GetChild(3).gameObject.SetActive(false);
                TrophyList[i].GetChild(4).gameObject.SetActive(false);
                TrophyList[i].GetChild(5).gameObject.SetActive(false);
            }
            else {
                TrophyList[i].GetChild(3).GetChild(0).GetComponent<Image>().sprite = DisabledTrophies[i*3];
                TrophyList[i].GetChild(3).GetChild(0).GetComponent<Image>().color = Color.black;
                TrophyList[i].GetChild(4).GetChild(0).GetComponent<Image>().sprite = DisabledTrophies[i*3+1];
                TrophyList[i].GetChild(4).GetChild(0).GetComponent<Image>().color = Color.black;
                TrophyList[i].GetChild(5).GetChild(0).GetComponent<Image>().sprite = DisabledTrophies[i*3+2];
                TrophyList[i].GetChild(5).GetChild(0).GetComponent<Image>().color = Color.black;
            }
            
        }
    }


    public void UpdateTrophyState()
    {
        GetTrohpyList(DataManager.MakeTrophyList(DataManager.MovesList));
    }

    string GetMonthText(int i)
    {
        if(i == 0) return "Jan";
        else if(i == 1) return "Feb";
        else if(i == 2) return "Mar";
        else if(i == 3) return "Apr";
        else if(i == 4) return "May";
        else if(i == 5) return "Jun";
        else if(i == 6) return "July";
        else if(i == 7) return "Aug";
        else if(i == 8) return "Sept";
        else if(i == 9) return "Oct";
        else if(i == 10) return "Nov";
        else return "Dec";
    }

    int GetMonthStars(int i )
    {
        if(i < 7)
        {
            if( i == 1)
            {
                return 140;
            }
            else if( i % 2 == 0)
            {
                return 155;
            }
            else
            {
                return 150;
            }
        }
        else
        {
            if( i % 2 == 0)
            {
                return 150;
            }
            else
            {
                return 155;
            }
        }
    }

    void GetTrohpyList(List<int> List)
    {
        MonthlyStarsList.Clear();

        for(int i = 0; i < List.Count; i++)
        {
            MonthlyStarsList.Add(List[i]);
        }


        SetTrophies();
    }

    public Sprite[] GetTrophyImage(int i)
    {
        Sprite[] images = new Sprite[3];

        images[0] = ActiveTrophies[i*3];
        images[1] = ActiveTrophies[i*3 + 1];
        images[2] = ActiveTrophies[i*3 + 2];

        return images;
    }


    void SetTrophies()
    {
        int month = StartDate.Month - 1;
        trophyCount[0] = 0;
        trophyCount[1] = 0;
        trophyCount[2] = 0;

        for(int i = 0; i < 12; i++)
        {
            if(i < month + MonthlyStarsList.Count && i >= month)
            {
                if(MonthlyStarsList[i - month] >= 15)
                {
                    TrophyList[i].GetChild(3).GetChild(0).GetComponent<Image>().sprite = ActiveTrophies[i*3];
                    TrophyList[i].GetChild(3).GetChild(0).GetComponent<Image>().color = Color.white;
                    trophyCount[0]++;
                }
                if(MonthlyStarsList[i - month] >= 60)
                {
                    TrophyList[i].GetChild(4).GetChild(0).GetComponent<Image>().sprite = ActiveTrophies[i*3+1];
                    TrophyList[i].GetChild(4).GetChild(0).GetComponent<Image>().color = Color.white;
                    trophyCount[1]++;
                }
                if(MonthlyStarsList[i - month] >= GetMonthStars(i))
                {
                    TrophyList[i].GetChild(5).GetChild(0).GetComponent<Image>().sprite = ActiveTrophies[i*3];
                    TrophyList[i].GetChild(5).GetChild(0).GetComponent<Image>().color = Color.white;
                    trophyCount[2]++;
                }
            }
        }
    }

    public void ShareTrophyStats()
    {
        string msg = "Check out my Dicele Trophies\n" + trophyCount[2].ToString() + " gold \n" + trophyCount[1].ToString() + "silver \n" + trophyCount[0].ToString() + "bronze\n#DiceleApp";
        DataManager.shareMessage = msg;
        DataManager.ShareText();
    }


}
