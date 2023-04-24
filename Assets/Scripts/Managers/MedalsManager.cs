using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedalsManager : MonoBehaviour
{
    
    [SerializeField] GameObject[] DailyMedals;
    [SerializeField] GameObject[] WeeklyMedals;

    DataManager DataManager;



    void Start()
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
        int rank = DataManager.PlayerDailyData.userRank;


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
                DailyMedals[i].transform.GetChild(2).gameObject.SetActive(true);
            }
        }

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
}
