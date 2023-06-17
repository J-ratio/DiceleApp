using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Calander : MonoBehaviour
{

    private DateTime StartDate = new DateTime(2023,1,10);
    private List<int> StateList = new List<int>();
    private List<int> StarsList = new List<int>();
    private List<int> MonthlyStarsList = new List<int>();

    [SerializeField]
    private TextMeshProUGUI TotalStarsCount;
    [SerializeField]
    private TextMeshProUGUI goldCheckPointTxt;
    [SerializeField]
    private TextMeshProUGUI ButtonMonthTxt;
    [SerializeField]
    private TextMeshProUGUI ButtonDayTxt;
    [SerializeField]
    private Button DailyDicele;
    [SerializeField]
    private Slider TrophySlider;

    [SerializeField]
    private GameObject backButton;
    [SerializeField]
    private GameObject nextButton;

    [SerializeField] Image[] Trophies;

    [SerializeField] TrophyManager TrophyManager;

    /// <summary>
    /// Cell or slot in the calendar. All the information each day should now about itself
    /// </summary>
    public class Day
    {
        public int dayNum;
        public int state;
        public int bestStarCount;
        public GameObject obj;
        public DayReferences dayRef;


        /// <summary>
        /// Constructor of Day
        /// </summary>
        public Day(int dayNum, int state, int bestStarCount, GameObject obj)
        {
            this.dayNum = dayNum;
            this.obj = obj;
            this.state = state;
            this.dayRef = obj.GetComponent<DayReferences>();
            this.bestStarCount = bestStarCount;
            UpdateState(state, bestStarCount);
            UpdateDay(dayNum);

        }

        /// <summary>
        /// Call this when updating the color so that both the state is updated, as well as the visual color on the screen
        /// </summary>
        public void UpdateState(int state, int bestStarCount)
        {
            this.state = state;
            this.bestStarCount = bestStarCount;

            for(int j = 0; j < 4; j++){
                dayRef.DayStates[j].SetActive(false);
            }

            if(state>=0){

                dayRef.DayStates[4].SetActive(false);
                dayRef.DayStates[state].SetActive(true);
            }


            if(state == 2)    dayRef.starText.text = bestStarCount.ToString();
            
        }

        /// <summary>
        /// When updating the day we decide whether we should show the dayNum based on the color of the day
        /// This means the color should always be updated before the day is updated
        /// </summary>
        public void UpdateDay(int newDayNum)
        {
            this.dayNum = newDayNum;
            if(state >= 0)
            {
                obj.GetComponent<DayReferences>().dayText.text = (dayNum + 1).ToString();

                if(state == 1 || state == 0)    dayRef.dayText.color = new Color(50/255f,50/255f,50/255f);
                else dayRef.dayText.color = Color.white;
            }
            else
            {
                obj.GetComponent<DayReferences>().dayText.text = "";
            }
        }
    }


    private List<Day> days = new List<Day>();

    public Transform[] weeks;

    public TextMeshProUGUI MonthAndYear;

    public DateTime currDate = DateTime.Now;

    private void OnEnable()
    {
        ActionEvents.StartCalander += StartCalander;
        ActionEvents.SendTrophyList += GetTrohpyList;
        DailyDicele.onClick.AddListener(delegate { StartLevel((DateTime.Now - StartDate).Days);});
    }


    void StartCalander(List<int> MovesList)
    {
        StateList.Clear();
        StarsList.Clear();
        
        for(int i = 0; i < MovesList.Count; i ++)
        {
            if(MovesList[i] == -2 )
            {
                StateList.Add(1);
                StarsList.Add(0);
            }
            else if( MovesList[i] == -1)
            {
                StateList.Add(4);
                StarsList.Add(0);
            }
            else
            {
                StateList.Add(2);
                StarsList.Add((21 - MovesList[i] > 5 ? 5 : 21 - MovesList[i]));
            }
        }

        ButtonMonthTxt.text = DateTime.Now.ToString("MMMM").Substring(0,3);
        ButtonDayTxt.text = DateTime.Now.Day.ToString();

        UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
    }


    void UpdateCalendar(int year, int month)
    {

        if(StartDate.Month == month) backButton.SetActive(false);
        else backButton.SetActive(true);

        if(DateTime.Now.Month == month) nextButton.SetActive(false);
        else nextButton.SetActive(true);


        Trophies[0].sprite = TrophyManager.GetTrophyImage(month - StartDate.Month)[0];
        Trophies[1].sprite = TrophyManager.GetTrophyImage(month - StartDate.Month)[1];
        Trophies[2].sprite = TrophyManager.GetTrophyImage(month - StartDate.Month)[2];


        DateTime temp = new DateTime(year, month, 1);
        currDate = temp;
        MonthAndYear.text = temp.ToString("MMMM") + " " + temp.Year.ToString();
        int startDay = GetMonthStartDay(year,month);
        int endDay = GetTotalNumberOfDays(year, month);

        goldCheckPointTxt.text = ((endDay - 1)*5).ToString();
        TotalStarsCount.text = MonthlyStarsList[((currDate.Year - StartDate.Year) * 12 + currDate.Month - StartDate.Month)].ToString();
        TrophySlider.value = GetSliderValue(((currDate.Year - StartDate.Year) * 12 + currDate.Month - StartDate.Month), (endDay - 1)*5);

        ///Create the days
        ///This only happens for our first Update Calendar when we have no Day objects therefore we must create them

        if(days.Count == 0)
        {
            for (int w = 0; w < 6; w++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Day newDay;
                    int currDay = (w * 7) + i;
                    if (currDay < startDay || currDay - startDay >= endDay)
                    {
                        newDay = new Day(currDay - startDay, -1,0,weeks[w].GetChild(i).gameObject);
                    }
                    else
                    {
                        if(DateTime.Compare(new DateTime(year,month,currDay - startDay + 1),StartDate) >= 0 && DateTime.Compare(new DateTime(year,month,currDay - startDay + 1),DateTime.Now) <= 0){
                            int tempIndex = (new DateTime(year,month,currDay - startDay + 1) - StartDate).Days;
                            newDay = new Day(currDay - startDay, StateList[tempIndex],StarsList[tempIndex],weeks[w].GetChild(i).gameObject);
                            weeks[w].GetChild(i).gameObject.GetComponent<Button>().enabled = true;
                            weeks[w].GetChild(i).gameObject.GetComponent<Button>().onClick.AddListener(delegate {StartLevel((new DateTime(year,month,currDay - startDay + 1) - StartDate).Days); });
                        }
                        else{
                            newDay = new Day(currDay - startDay, 0,0,weeks[w].GetChild(i).gameObject);
                            weeks[w].GetChild(i).gameObject.GetComponent<Button>().enabled = false;
                        }
                    
                    }
                    days.Add(newDay);
                }
            }
        }
        ///loop through days
        ///Since we already have the days objects, we can just update them rather than creating new ones
        else
        {
            for(int i = 0; i < 42; i++)
            {

                days[i].obj.GetComponent<Button>().onClick.RemoveAllListeners();

                if(i < startDay || i - startDay >= endDay)
                {
                    for(int j = 0; j < 4; j++)
                    {
                        days[i].obj.GetComponent<DayReferences>().DayStates[j].SetActive(false);
                    }

                    days[i].UpdateState(-1,0);
                }
                else
                {
                    if(DateTime.Compare(new DateTime(year,month,i - startDay + 1),StartDate) >= 0 && DateTime.Compare(new DateTime(year,month,i - startDay + 1),DateTime.Now) <= 0)
                    {
                        DateTime tempD =  new DateTime(year,month,i - startDay + 1);
                        days[i].UpdateState(StateList[(tempD - StartDate).Days],StarsList[(tempD - StartDate).Days]);
                        days[i].obj.GetComponent<Button>().enabled = true;
                        days[i].obj.GetComponent<Button>().onClick.AddListener(delegate {StartLevel((tempD - StartDate).Days); });
                    }
                    else
                    {
                        days[i].UpdateState(0,0);
                        days[i].obj.GetComponent<Button>().enabled = false;
                    }
                }
                
                days[i].UpdateDay(i - startDay);
            }
        }

        ///This just checks if today is on our calendar.
        if(DateTime.Now.Year == year && DateTime.Now.Month == month)
        {
            if(StateList[StateList.Count - 1] == 1){
            days[(DateTime.Now.Day - 1) + startDay].UpdateState(3,0);
            }
        }

    }


    float GetSliderValue(int index, int TotalStars)
    {
        if(index < MonthlyStarsList.Count && index >= 0)
        {
            if(MonthlyStarsList[index] < 15) return (MonthlyStarsList[index]/15f)*0.33f;
            else if(MonthlyStarsList[index] < 60) return ((MonthlyStarsList[index]-15)/45f)*0.33f + 0.33f;
            else return ((float)(MonthlyStarsList[index]-60)/(TotalStars-60))*0.33f + 0.66f;
        }
        else
        {
            return 0;
        }

    }


    void LogList(List<int> List)
    {
        string s = "";

        foreach( int i in List)
        {
            s += i + " ";
        }

        Debug.Log(s);
    }


    void RemoveListeners()
    {
        for (int w = 0; w < 6; w++)
        {
            for (int i = 0; i < 7; i++)
            {
                weeks[w].GetChild(i).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }



    void StartLevel(int Num)
    {
        ActionEvents.StartLvl(Num);
    }



    /// <summary>
    /// This returns which day of the week the month is starting on
    /// </summary>

    int GetMonthStartDay(int year, int month)
    {
        DateTime temp = new DateTime(year, month, 1);

        //DayOfWeek Sunday == 0, Saturday == 6 etc.
        return (int)temp.DayOfWeek;
    }




    /// <summary>
    /// Gets the number of days in the given month.
    /// </summary>

    int GetTotalNumberOfDays(int year, int month)
    {
        return DateTime.DaysInMonth(year, month);
    }



    /// <summary>
    /// This either adds or subtracts one month from our currDate.
    /// The arrows will use this function to switch to past or future months
    /// </summary>

    public void SwitchMonth(bool direction)
    {
        if(direction)
        {
            currDate = currDate.AddMonths(1);
        }
        else
        {
            currDate = currDate.AddMonths(-1);
        }

        UpdateCalendar(currDate.Year, currDate.Month);
    }


    void GetTrohpyList(List<int> List)
    {
        MonthlyStarsList.Clear();

        for(int i = 0; i < List.Count; i++)
        {
            MonthlyStarsList.Add(List[i]);
        }

        LogList(List);
    }


    void OnDisable()
    {
        ActionEvents.StartCalander -= StartCalander;
        ActionEvents.SendTrophyList -= GetTrohpyList;
        DailyDicele.onClick.RemoveAllListeners();
    }
}
