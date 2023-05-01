using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;


public class DataManager : MonoBehaviour
{
    

    private List<int> solList = new List<int>();
    private List<int> spawnList = new List<int>();
    private int gameIndex;
    public int ClassicLvlIndex;
    private int currentUndoNum;
    private int currentHintNum;

    private bool isDaily = true;
    private bool isNewDay = false;
    private bool isNextDay = false;

    [SerializeField] GameObject loadingScreen;

    DateTime StartDate = new DateTime(2023,1,10);

    // # of days since start date
    private int DayNum;

    // # of month since start date
    private int MonthNum;


    //stores the board dice values of the current ongoing game 
    //later to be used as a fallback feature in case player disconnects
    private List<int> currStateList;


    public string userName;
    private List<int> ScoreList;
    public List<int> MovesList;
    private List<int> TrophyList;
    private int PlayerXp = 0;
    private int PlayerCoins = 0;
    public int BestRank = 0;
    public int BestStreak = 0;
    public int ActiveStreak = 0;
    public int ClassicStreak;
    public int BestClassicStreak;
    int[] piggyArr = {3,8,15,25,40,60,90,150,250};
    int[] queuedWinCase = new int[4];




    private UIManager UIManager;
    public CoinsManager coinAnim;
    public WaitSlider slider;


    private int[] tempExampleClassicSolArr0 = { 3, 1, 0, 5 };
    private int[] tempExampleClassicSpawnArr0 = { 5, 1, 0, 3 };
    private int[] tempExampleClassicSolArr1 = { 0, 1, 4, 1, -1, 0, 5, 0, 2 };
    private int[] tempExampleClassicSpawnArr1 = { 2, 1, 0, 1, -1, 0, 5, 0, 4 };

    [SerializeField] private GamePlayTutorial playTutorial;//Added by charan


    // Database URLs

    string getURL = "http://api.dicele.com";
    string postURL = "http://api.dicele.com";

    string registerIDURL = "/user/login";
    string userProfileURL = "/user/profile/";
    string updateStatsURL = "/update/stats";
    string userDailyDataURL = "/user/";
    string userDailyLeaderboardsURL = "/leaderboard/daily/";
    string userWeeklyLeaderboardsURL = "/leaderboard/weekly/";
    string userAllTimeLeaderboardsURL = "/leaderboard/alltime/";


    


    void Awake()
    {
        userProfileURL += SystemInfo.deviceUniqueIdentifier;
        userDailyLeaderboardsURL += SystemInfo.deviceUniqueIdentifier;
        userWeeklyLeaderboardsURL += SystemInfo.deviceUniqueIdentifier;
        userAllTimeLeaderboardsURL += SystemInfo.deviceUniqueIdentifier;
        userDailyDataURL += SystemInfo.deviceUniqueIdentifier + "/daily";

    }

    [Serializable]
    class IdClass
    {
        public string deviceId = SystemInfo.deviceUniqueIdentifier;
    }

    [Serializable]
    class ScoreClass
    {
        public int score;
        public string diceleId;
    }



    //_____________________________________________________________________________
    [Serializable]
    public class GetDailyRankResponse{

        public Dailyinfo info;
    }

    [Serializable]
    public class Dailyinfo{

            public currentDay currentDay;
            public yesterday yesterday;
    }


    [Serializable]
    public class GetWeeklyRankResponse{

        public Weeklyinfo info;
    }

    [Serializable]
    public class Weeklyinfo{

        public userStats userStats;
        public userStats[] leaderboard;
    }

    [Serializable]
    public class GetAllTimeRankResponse{
        public Weeklyinfo info;
    }


    [Serializable]
    public class currentDay{

        public userStats userStats;
        public userStats[] leaderboard;

    }


    [Serializable]
    public class yesterday{
        public userStats userStats;
        public userStats[] leaderboard;
    }


    [Serializable]
    public struct  userStats{   
        
        public string userName;
        public int score;
        public int allTimeScore;
        public int userRank;
    }

    //______________________________________________________________________________


    public List<userStats> DailyRankList = new List<userStats>(10);
    public List<userStats> DailyTopRankList = new List<userStats>(3);
    public List<userStats> WeeklyRankList = new List<userStats>(10);
    public List<userStats> AllTimeRankList = new List<userStats>(10);
    public userStats PlayerDailyData;
    public userStats PlayerYesterdayData;
    public userStats PlayerWeeklyData;
    public userStats PlayerAllTimeData;



    async void StoreDailyBoard()
    {
        var temp = await ApiHelper.RequestJSONData(getURL+userDailyLeaderboardsURL);
        GetDailyRankResponse UserDailyData = JsonUtility.FromJson<GetDailyRankResponse>(temp.response);
        Debug.Log(temp.response);

        for(int i = 0; i < 10; i++)
        {
            if(i < UserDailyData.info.currentDay.leaderboard.Count())
            {
                DailyRankList.Add(UserDailyData.info.currentDay.leaderboard[i]);
            }
        }

        for(int i = 0; i < 3; i++)
        {
            if(i < UserDailyData.info.yesterday.leaderboard.Count())
            {
                DailyTopRankList.Add(UserDailyData.info.yesterday.leaderboard[i]);
            }

        }

        PlayerDailyData = UserDailyData.info.currentDay.userStats;
        PlayerYesterdayData = UserDailyData.info.yesterday.userStats;
        
    }


    async void StoreWeeklyBoard()
    {

        var temp = await ApiHelper.RequestJSONData(getURL+userWeeklyLeaderboardsURL);
        GetWeeklyRankResponse UserWeeklyData = JsonUtility.FromJson<GetWeeklyRankResponse>(temp.response);
        Debug.Log(temp.response);

        for(int i = 0; i < 10; i++)
        {
            if(i < UserWeeklyData.info.leaderboard.Count())
            {
                WeeklyRankList.Add(UserWeeklyData.info.leaderboard[i]);
            }
            
        }

        PlayerWeeklyData = UserWeeklyData.info.userStats;

    }


    async void StoreAllTimeBoard()
    {
        var temp = await ApiHelper.RequestJSONData(getURL+userAllTimeLeaderboardsURL);
        GetAllTimeRankResponse UserAllTimeData = JsonUtility.FromJson<GetAllTimeRankResponse>(temp.response);
        Debug.Log(temp.response);


        for(int i = 0; i < 10; i++)
        {
            if(i < UserAllTimeData.info.leaderboard.Count())
            {
                AllTimeRankList.Add(UserAllTimeData.info.leaderboard[i]);
            }
            
        }

        PlayerAllTimeData = UserAllTimeData.info.userStats;
    }



    void OnEnable()
    {
        ActionEvents.StartLvl += StartLvl;
        ActionEvents.TriggerGameWinEvent += GameWinEvent;
        ActionEvents.TriggerGameLoseEvent += GameLoseEvent;
        ActionEvents.UpdateGameState += UpdateGameState;

        UIManager = GetComponent<UIManager>();
    }


    [Serializable]
    public class InitClass
    {
        public int[] scoreList;
        public int[] movesList;
        public int classicIndex;
        public int playerCoins;
        public int playerXp;
        public int BestRank;
        public int BestStreak;
        public int ActiveStreak;
        public int ClassicStreak;
        public int BestClassicStreak;
        public int Avatar;
    }

    [Serializable]
    class DailyDataClass
    {
        public int[] scoreList;
        public int[] movesList;
    }


    [Serializable]
    class CoinsClass
    {
        public int playerCoins;
    }

    [Serializable]
    class XpClass
    {
        public int playerXp;
    }

    [Serializable]
    class ClassicIndexClass
    {
        public int classicIndex;
    }


    [Serializable]
    class StreakClass
    {
        public int BestStreak;
        public int ActiveStreak;
    }

    [Serializable]
    class ClassicStreakClass
    {
        public int ClassicStreak;
        public int BestClassicStreak;
    }


    [Serializable]
    class RankClass
    {
        public int BestRank;
    }


    [Serializable]
    class NameClass
    {
        public string userName;
    }


    [Serializable]
    class AvatarClass
    {
        public int Avatar;
    }


    [Serializable]
    public class GetResponse {

        public InfoClass info;

    }


    [Serializable]
    public class GetDailyResponse {

        public DailyInfoClass info;

    }


    [Serializable]
    public class InfoClass
    {
        public string userName;
        public List<int> scoreList;
        public List<int> movesList;
        public int classicIndex;
        public int playerCoins;
        public int playerXp;
        public int BestRank;
        public int ActiveStreak;
        public int BestStreak;
        public int ClassicStreak;
        public int BestClassicStreak;
        public int Avatar;
    }


    [Serializable]
    public class DailyInfoClass
    {
        public int score;
        public dailyDiceleClass dicele;
    }

    [Serializable]
    public class dailyDiceleClass
    {
        public string _id;
    }



    /***********************************************/

    [Serializable]
    public class GetRankResponse {

        public info info;
    }

    [Serializable]
    public class info{

            public currentDayClass currentDay;
    }

    [Serializable]
    public class currentDayClass{

        public Rank userStats;

    }

    [Serializable]
    public struct  Rank{   
        
        public int userRank;
    }

    /************************************************/



    async void Start()
    {  

        
        bool isFirstTime = false;

        DayNum = (DateTime.Now - StartDate).Days;
        MonthNum = (DateTime.Now.Month - StartDate.Month) + 12 * (DateTime.Now.Year - StartDate.Year);


        ScoreList = new List<int>(DayNum + 1);
        MovesList = new List<int>(DayNum + 1);
        TrophyList = new List<int>(MonthNum);

        Debug.Log("Started");
        
        //login user
        IdClass Object = new IdClass();
        var temp0 = await ApiHelper.SendJSONData(postURL+registerIDURL, Object);
        GetResponse UserData = JsonUtility.FromJson<GetResponse>(temp0.response);

        userName = UserData.info.userName;
        UIManager.UpdateName(userName);


        //LogList(UserData.info.scoreList);
        

        //_____________________________________________________________________________________________
        //retrieves the score,moves from the database and stores them into the above Lists.
        //_____________________________________________________________________________________________

        if(UserData.info.scoreList.Count() == 0)
        {
            isFirstTime = true;
            ScoreList = Enumerable.Repeat(0, DayNum + 1).ToList();
            MovesList = Enumerable.Repeat(-2, DayNum + 1).ToList();

            InitClass tempObject = new InitClass();
            tempObject.scoreList = ScoreList.ToArray();
            tempObject.movesList = MovesList.ToArray();
            tempObject.playerCoins = 0;
            tempObject.playerXp = 0;
            tempObject.classicIndex = 0;
            tempObject.BestRank = 0;
            tempObject.BestStreak = 0;
            tempObject.ActiveStreak = 0;
            tempObject.Avatar = 0;
            tempObject.ClassicStreak = 0;
            tempObject.BestClassicStreak = 0;

            temp0 = await ApiHelper.SendJSONData(postURL + userProfileURL + updateStatsURL, tempObject);
            UserData = JsonUtility.FromJson<GetResponse>(temp0.response);


            isNewDay = true;
            isNextDay = true;
        }
        else
        {
            
            int[] tempScoreArr = UserData.info.scoreList.ToArray();
            int[] tempMovesArr = UserData.info.movesList.ToArray();

            if(tempScoreArr.Count() < DayNum+1)
            {
                isNewDay = true;
                if(tempScoreArr.Count() == DayNum)
                {
                    if(tempMovesArr[DayNum-1] >= 0)
                    {
                        isNextDay = true;
                    }
                }
                else
                {
                    ActiveStreak = 0;
                    //______________
                    //Update var
                    StreakClass Object1 = new StreakClass();
                    Object1.BestStreak = BestStreak;
                    Object1.ActiveStreak = ActiveStreak;
                    await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object1);
                    //______________
                }

                
            }

            for(var i = 0; i < DayNum+1; i++)
            {

                if(i < tempScoreArr.Count())
                {
                    //ScoreList[i] = tempScoreArr[i];
                    //MovesList[i] = tempMovesArr[i];
                    ScoreList.Add(tempScoreArr[i]);
                    MovesList.Add(tempMovesArr[i]);
                }
                else
                {
                    //ScoreList[i] = 0;
                    //MovesList[i] = -2;
                    ScoreList.Add(0);
                    MovesList.Add(0);
                }
            }   
            
            DailyDataClass Object0 = new DailyDataClass();
            Object0.scoreList = ScoreList.ToArray();
            Object0.movesList = MovesList.ToArray();

            temp0 = await ApiHelper.SendJSONData(postURL + userProfileURL + updateStatsURL, Object0);
            UserData = JsonUtility.FromJson<GetResponse>(temp0.response);

            Debug.Log("passed init2");
            
        }



        //Setting all elements to zero for testing purpose
        //ScoreList = Enumerable.Repeat(0, DayNum + 1).ToList();
        //MovesList = Enumerable.Repeat(-2, DayNum + 1).ToList();

        //_____________________________________________________________________________________________
        //retrieves playerInfo from the database and stores them into the given variables.
        //_____________________________________________________________________________________________
        ClassicLvlIndex = UserData.info.classicIndex;
        PlayerXp = UserData.info.playerXp;
        PlayerCoins = UserData.info.playerCoins;
        BestRank = UserData.info.BestRank;
        ActiveStreak = UserData.info.ActiveStreak;
        BestStreak = UserData.info.BestStreak;
        ClassicStreak = UserData.info.ClassicStreak;
        BestClassicStreak = UserData.info.BestClassicStreak;
        UIManager.UpdatePlayerStats(PlayerXp, PlayerCoins);
        UIManager.UpdateClassicLevel(ClassicLvlIndex + 1);
        UIManager.UpdateAvatar(UserData.info.Avatar);


        //get daily and weekly ranks from leaderboard
        StoreDailyBoard();
        StoreWeeklyBoard();
        StoreAllTimeBoard();

        UpdateStats();
        slider.SetSliderToFull();

        transform.GetComponent<GoogleAdMobController>().RequestBannerAd();

        if(isFirstTime) StartClassicLvl();

    }


    public async void ChangeUserName()
    {
        UIManager.UpdateName(UIManager.UserNameInputText.text);
        NameClass Object = new NameClass();
        Object.userName = UIManager.UserNameInputText.text;
        await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object);
    }


    public async void UpdateAvatar(int num)
    {
        UIManager.UpdateAvatar(num);
        AvatarClass Object = new AvatarClass();
        Object.Avatar = num;
        await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object);
    }



    public void UpdateStats()
    {
        int[] StatsArr = Enumerable.Repeat(0, 8).ToArray();

        StatsArr[0] = MovesList.Count(x => x >= -1);
        StatsArr[1] = BestRank;
        StatsArr[2] = ActiveStreak;
        StatsArr[3] = BestStreak;

        for(int i = 0; i < MovesList.Count; i ++)
        {
            if(MovesList[i] >= 0 )
            {
                StatsArr[4] +=  (21 - MovesList[i] > 5 ? 5 : 21 - MovesList[i]);
            }
        }

        if(StatsArr[0]!=0)
        {
            StatsArr[5] = (MovesList.Count(x => x >= 0)*100/StatsArr[0]);
        }
        else
        {
            StatsArr[5] = 100;
        }

        for(int i = 0; i < ScoreList.Count; i++)
        {
            StatsArr[6] += ScoreList[i];
        }
        

        int positive = MovesList.Count(x => x>= 0);

        if(positive != 0)
        {
            StatsArr[6] /= MovesList.Count(x => x >= 0);
        }
        else
        {
            StatsArr[6] = 0;
        }

        StatsArr[7] = ScoreList.Max();


        UIManager.UpdateStatsScreenUI(StatsArr);

    }



    /// <summary>
    /// This function is called when user clicks on the daily dicele button.
    /// </summary>

    public void StartCalander()
    {
        if (isDaily)
        {
            UIManager.OpenScreen("Calander_Canvas");
            ActionEvents.SendTrophyList(MakeTrophyList(MovesList));
            ActionEvents.StartCalander(MovesList);
        }
        else
        {
            isDaily = true;
            UIManager.OpenScreen("Main_Menu_Canvas");
        }
    }



    /// <summary>
    /// This function is called when user clicks on a calander button to start a new game.
    /// </summary>

    void StartLvl(int levelNum)
    {
        playTutorial.isClassic = false;//Added by charan
        isDaily = true;
        gameIndex = levelNum;
        currentUndoNum = 0;
        currentHintNum = 0;
        
        //________________________________________________________________________________________________
        //fetches nth(gameIndex) array of Solution and Spawn GameData fields from database and stores in solList and spawnList
        //________________________________________________________________________________________________
        StoreGameData(Enumerable.Range(0, GameData.DailySpawn.GetLength(1)).Select(x => GameData.DailySpawn[gameIndex,x]).ToArray(), Enumerable.Range(0, GameData.DailySol.GetLength(1)).Select(x => GameData.DailySol[gameIndex,x]).ToArray());

        currStateList = new List<int>(spawnList);

        UIManager.UpdateGameScreen(PlayerCoins, StartDate.AddDays(gameIndex).Day, StartDate.AddDays(gameIndex).ToString("MMMM").Substring(0, 3), 100, 50);


        //Opens the game canvas
        UIManager.OpenScreen("GamePlay_Canvas");
        UIManager.CloseScreen("Calander_Canvas");


        //Sends the Lists to gameBoard
        ActionEvents.SendGameData(solList, spawnList, 0, isDaily);


    }


    public void StartClassicLvl()
    {
        playTutorial.isClassic = true;//Added by charan
        isDaily = false;
        currentUndoNum = 0;
        currentHintNum = 0;
        //________________________________________________________________________________________________
        //fetches nth(ClassicLvlIndex) array of Solution and Spawn GameData fields from database and stores in solList and spawnList
        //________________________________________________________________________________________________

        if (ClassicLvlIndex == 0) StoreGameData(tempExampleClassicSpawnArr0, tempExampleClassicSolArr0);
        else if (ClassicLvlIndex == 1) StoreGameData(tempExampleClassicSpawnArr1, tempExampleClassicSolArr1);
        else if(ClassicLvlIndex < 8) StoreGameData(Enumerable.Range(0, GameData.ClassicSpawnSmall.GetLength(1)).Select(x => GameData.ClassicSpawnSmall[ClassicLvlIndex-2,x]).ToArray(), Enumerable.Range(0, GameData.ClassicSolSmall.GetLength(1)).Select(x => GameData.ClassicSolSmall[ClassicLvlIndex -2,x]).ToArray());
        else if(ClassicLvlIndex < 17) StoreGameData(Enumerable.Range(0, GameData.ClassicSpawnMedium.GetLength(1)).Select(x => GameData.ClassicSpawnMedium[ClassicLvlIndex-8,x]).ToArray(), Enumerable.Range(0, GameData.ClassicSolMedium.GetLength(1)).Select(x => GameData.ClassicSolMedium[ClassicLvlIndex - 8,x]).ToArray());
        else StoreGameData(Enumerable.Range(0, GameData.ClassicSpawnLarge.GetLength(1)).Select(x => GameData.ClassicSpawnLarge[ClassicLvlIndex-17,x]).ToArray(), Enumerable.Range(0, GameData.ClassicSolLarge.GetLength(1)).Select(x => GameData.ClassicSolLarge[ClassicLvlIndex - 17,x]).ToArray());
        
        

        //StoreGameData(spawnList, solList);

        currStateList = new List<int>(spawnList);

        UIManager.UpdateGameScreen(PlayerCoins, ClassicLvlIndex + 1, "Level", 100, 50);

        //Opens the game canvas
        UIManager.OpenScreen("GamePlay_Canvas");

        //Sends the Lists to gameBoard
        ActionEvents.SendGameData(solList, spawnList, 0, isDaily);
    }

    private void StoreGameData(int[] spawn, int[] sol)
    {
        solList.Clear();
        spawnList.Clear();

        for (int i = 0; i < spawn.Count(); i++)
        {
            solList.Add(sol[i]);
            spawnList.Add(spawn[i]);
        }
    }

    private void LogList(List<int> List)
    {
        string s = "";
        foreach(int i in List)
        {
            s += i + " ";
        }

        Debug.Log(s);
    }

    public List<int> MakeTrophyList(List<int> MovesList)
    {
        List<int> StarsList = new List<int>();
        List<int> MonthlyStarsList = new List<int>();

        for(int i = 1; i < MovesList.Count + 1; i ++)
        {
            if(MovesList[MovesList.Count - i] == -2)
            {
                StarsList.Add(0);
            }
            else if( MovesList[MovesList.Count - i] == -1)
            {
                StarsList.Add(0);
            }
            else
            {
                StarsList.Add((21 - MovesList[MovesList.Count - i] > 5 ? 5 : 21 - MovesList[MovesList.Count - i]));
            }
        }


        //LogList(MovesList);
        //LogList(StarsList);


        DateTime now = StartDate;
        DateTime endOfMonth = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
        TimeSpan timeLeft = endOfMonth - now;
        int daysLeft = (int)timeLeft.TotalDays;

        MonthlyStarsList.Add(0);
        for(int i = 0; i < daysLeft+1; i++)
        {
            MonthlyStarsList[0] += StarsList[StarsList.Count - 1];
            StarsList.RemoveAt(StarsList.Count - 1);
        }

        //LogList(MonthlyStarsList);

        DateTime endDate = DateTime.Now;
        int months = (endDate.Year - StartDate.Year) * 12 + endDate.Month - StartDate.Month;

        int tempMonth = 0;
        while(months > 0)
        {
            tempMonth++;
            int daysInMonth = DateTime.DaysInMonth(StartDate.Year, StartDate.Month + tempMonth);

            MonthlyStarsList.Add(0);
            for(int i = 0; i < daysInMonth; i++)
            {
                if(StarsList.Count > 0)
                {
                    MonthlyStarsList[tempMonth] += StarsList[StarsList.Count - 1];
                    StarsList.RemoveAt(StarsList.Count - 1);
                }
            }
            months--;

            
        }


        return MonthlyStarsList;
    }


    public async void Undo()
    {
        int[] CoinOptions = { 50, 100, 300 };
        int requiredCoins = CoinOptions[currentUndoNum > 2 ? 2 : currentUndoNum];

        if (PlayerCoins < requiredCoins)
        {
            UIManager.OpenScreen("In app_coin_Canvas");
            return;
        }

        //Debug.Log("Checking...");

        if (currentUndoNum < 3)
        {
            //Debug.Log("Swapping...");
            currentUndoNum++;
            PlayerCoins = PlayerCoins - requiredCoins;
            UIManager.UpdatePlayerStats(PlayerXp, PlayerCoins);
            UIManager.GameScreenUI[3].text = CoinOptions[currentUndoNum > 1 ? 2 : currentUndoNum].ToString();
            ActionEvents.Undo();

            //-----------------------------------------------------------------------
            //Update Player Coins
            CoinsClass Object = new CoinsClass();
            Object.playerCoins = PlayerCoins;
            await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object);
            //-----------------------------------------------------------------------
            
        }
    }



    public async void Hint()
    {
        int[] CoinOptions = { 100, 200, 500, 1000 };
        int requiredCoins = CoinOptions[currentHintNum > 3 ? 3 : currentHintNum];

        if (PlayerCoins < requiredCoins)
        {
            UIManager.OpenScreen("In app_coin_Canvas");
            return;
        }

        if (currentHintNum < 4)
        {
            currentHintNum++;
            PlayerCoins = PlayerCoins - requiredCoins;
            UIManager.UpdatePlayerStats(PlayerXp, PlayerCoins);
            UIManager.GameScreenUI[4].text = CoinOptions[currentHintNum > 2 ? 3 : currentHintNum].ToString();
            ActionEvents.Hint();

            //-----------------------------------------------------------------------
            //Update Player Coins
            CoinsClass Object = new CoinsClass();
            Object.playerCoins = PlayerCoins;
            await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object);
            //-----------------------------------------------------------------------

        }
    }



    /// <summary>
    /// This function is called when user selects the option Add 3 moves
    /// </summary>

    public void StartCustomLevel()
    {
        currentUndoNum = 0;
        currentHintNum = 0;
        //________________________________________________________________________________________________
        //fetches nth(gameIndex) array of Solution GameData fields from database and stores in solList
        if(isDaily)
        {
            StoreGameData(currStateList.ToArray(), Enumerable.Range(0, GameData.DailySol.GetLength(1)).Select(x => GameData.DailySol[gameIndex,x]).ToArray());
        }
        else
        {
            if (ClassicLvlIndex == 0) StoreGameData(currStateList.ToArray(), tempExampleClassicSolArr0);
            else if (ClassicLvlIndex == 1) StoreGameData(currStateList.ToArray(), tempExampleClassicSolArr1);
            else if(ClassicLvlIndex < 8) StoreGameData(currStateList.ToArray(), Enumerable.Range(0, GameData.ClassicSolSmall.GetLength(1)).Select(x => GameData.ClassicSolSmall[ClassicLvlIndex -2,x]).ToArray());
            else if(ClassicLvlIndex < 17) StoreGameData(currStateList.ToArray(), Enumerable.Range(0, GameData.ClassicSolMedium.GetLength(1)).Select(x => GameData.ClassicSolMedium[ClassicLvlIndex - 8,x]).ToArray());
            else StoreGameData(currStateList.ToArray(), Enumerable.Range(0, GameData.ClassicSolLarge.GetLength(1)).Select(x => GameData.ClassicSolLarge[ClassicLvlIndex - 17,x]).ToArray());    
        }
        //________________________________________________________________________________________________

        //Opens the game canvas
        UIManager.OpenScreen("GamePlay_Canvas");
        UIManager.CloseScreen("Level_Failed_Canvas");


        //Sends the Lists to gameBoard 
        ActionEvents.SendGameData(solList, spawnList, 1, isDaily);
    }


    private void UpdateGameState(int x, int y)
    {
        int temp = currStateList[x];
        currStateList[x] = currStateList[y];
        currStateList[y] = temp;
    }


    async void GameWinEvent(int swapped, int playtime)
    {

        // TimeBonus + MoveBonus + WinningBonus(1000)
        int score = ((playtime < 50) ? 500 : (playtime > 290 ? 20 : (300 - playtime) * 2)) + (21 - swapped) * 100 + 1000;
        int xp = 0;
        int coins;
        int rank = 0;

        if (isDaily)
        {
            coins = (xp - 10 > 30 ? (xp - 10) / 100 : 30);
            // Winning Bonus(10) + ScoreImprovementBonus
            xp = 10 + ((score - ScoreList[gameIndex]) > 0 ? (score - ScoreList[gameIndex]) : 0) / 100;
        }
        else
        {
            coins = 10;
            xp = 10 + (score) / 100;
        }


        queuedWinCase[0] = coins;
        queuedWinCase[1] = xp;
        queuedWinCase[2] = swapped;
        queuedWinCase[3] = score;

        //Show Results
        if (gameIndex == DayNum && isDaily)
        {
            loadingScreen.SetActive(true);
            //_____________________________________________________________________________________________
            //fetches the rank for the above score value

            var temp = await ApiHelper.RequestJSONData(getURL+userDailyDataURL);
            Debug.Log(temp.response);
            GetDailyResponse UserDailyData = JsonUtility.FromJson<GetDailyResponse>(temp.response);
            int tempScore = UserDailyData.info.score;
            string diceleID = UserDailyData.info.dicele._id;
            Debug.Log(diceleID);


            ScoreClass Object = new ScoreClass();
            Object.score = score;
            Object.diceleId = diceleID;
            var t = await ApiHelper.SendJSONData(postURL+userDailyDataURL, Object);
            Debug.Log(t.response);


            temp = await ApiHelper.RequestJSONData(getURL+userDailyLeaderboardsURL);
            GetRankResponse UserDailyData0 = JsonUtility.FromJson<GetRankResponse>(temp.response);

            Object.score = tempScore;
            Object.diceleId = diceleID;
            await ApiHelper.SendJSONData(postURL+userDailyDataURL, Object);
            //_____________________________________________________________________________________________

            rank = UserDailyData0.info.currentDay.userStats.userRank;
            UIManager.UpdateResultScreen(score, rank, xp, coins, true);

            if(rank > BestRank)
            {
                BestRank = rank;
                //________________
                //Update BestRank field in db
                RankClass Object0 = new RankClass();
                Object0.BestRank = BestRank;
                await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object0);
                //________________
            }

            if(isNextDay)
            {
                ActiveStreak += 1;
                if(BestStreak < ActiveStreak)
                {
                    BestStreak = ActiveStreak;
                }

                //______________________________
                //Update Streaks
                StreakClass Object1 = new StreakClass();
                Object1.BestStreak = BestStreak;
                Object1.ActiveStreak = ActiveStreak;
                await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object1);
                //______________________________
            }

            loadingScreen.SetActive(false);
        }
        else
        {
            UIManager.UpdateResultScreen(score, rank, xp, coins, false);
        }

        if (isDaily)
        {
            UIManager.OpenScreen("Level_Complete_Canvas");
        }
        else if(ClassicLvlIndex != 0 && ClassicLvlIndex != 1)
        {
            UIManager.OpenScreen("Level_Complete_Classic_Canvas");
        }
        else
        {
            UIManager.OpenScreen("Level_TutComplete_Canvas");
        }

        UIManager.CloseScreen("GamePlay_Canvas");

        if(isDaily)
        {
            UpdateStats();
        }



        if(gameIndex == DayNum && isDaily)
        {
            StoreAllTimeBoard();
            StoreWeeklyBoard();
            StoreDailyBoard();
        }
        
    }

    private async void GameLoseEvent()
    {

        if (isDaily)
        {
            MovesList[gameIndex] = -1;            
            UpdateStats();

            UIManager.OpenScreen("Level_Failed_Canvas");
            UIManager.CloseScreen("GamePlay_Canvas");

            
            if(gameIndex == DayNum)
            {
                if(isNextDay)
                {
                    ActiveStreak = 0;

                    //_________________________
                    //UpdateStreak
                    StreakClass Object1 = new StreakClass();
                    Object1.BestStreak = BestStreak;
                    Object1.ActiveStreak = ActiveStreak;
                    await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object1);
                    //_________________________
                }

            }

            //_____________________________________________________________________________________________
            //stores -1 in DataBase's moveslist at position gameIndex
            DailyDataClass Object0 = new DailyDataClass();
            Object0.scoreList = ScoreList.ToArray();
            Object0.movesList = MovesList.ToArray();

            await ApiHelper.SendJSONData(postURL + userProfileURL + updateStatsURL, Object0);
            //_____________________________________________________________________________________________
        }
        else
        {
            UIManager.OpenScreen("Level_Failed_Canvas");
            UIManager.CloseScreen("GamePlay_Canvas");
        }
    }


    public void ShowPiggyLostScreen()
    {

        int winsAwayFromReward = 0;
        int goldNum = 0;

        for(int i = 0; i < piggyArr.Count(); i++)
        {   
            
            if(ClassicStreak < piggyArr[i])
            {
                winsAwayFromReward = piggyArr[i] - ClassicStreak;
                goldNum = ClassicStreak*10;
                break;
            }
        }

        UIManager.UpdatePiggyScreen(goldNum, winsAwayFromReward, 2);

        UIManager.OpenScreen("PiggyBankLost_Canvas");
    }


    public async void LoseStreak()
    {
        ClassicStreak = 0;
        //________________________________________________________________________
        ClassicStreakClass Object2 = new ClassicStreakClass();
        Object2.ClassicStreak = ClassicStreak;
        Object2.BestClassicStreak = BestClassicStreak;
        await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object2);
        //________________________________________________________________________
    }


    public async void AwardGold()
    {
        PlayerCoins += 30;
        UIManager.UpdatePlayerStats(PlayerXp, PlayerCoins);
        coinAnim.AddCoins(0);

        //-----------------------------------------------------------------------
            //Update Player Coins
            CoinsClass Object = new CoinsClass();
            Object.playerCoins = PlayerCoins;
            await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object);
        //-----------------------------------------------------------------------

    }


    public void RetryLvl()
    {
        if (isDaily)
        {
            StartLvl(gameIndex);
        }
        else
        {
            StartClassicLvl();
        }
    }


    public async void Collect(bool doubleReward)
    {

        int score = queuedWinCase[3];
        int swapped = queuedWinCase[2];

        if(doubleReward) {
            PlayerCoins = PlayerCoins + 2*queuedWinCase[0];
            PlayerXp = PlayerXp + 2*queuedWinCase[1];
        }
        else {
            PlayerCoins = PlayerCoins + queuedWinCase[0];
            PlayerXp = PlayerXp + queuedWinCase[1];
        }

        UIManager.UpdatePlayerStats(PlayerXp, PlayerCoins);



        UIManager.CloseScreen("Level_Complete_Canvas");
        UIManager.CloseScreen("Level_Complete_Classic_Canvas");
        UIManager.CloseScreen("Level_Failed_Canvas");


        //Updating Database
        if (isDaily)
        {

            if (ScoreList[gameIndex] < score)
            {
                ScoreList[gameIndex] = score;
                //_____________________________________________________________________________________________
                //stores the score value in DataBase's scorelist at position gameIndex       
                //_____________________________________________________________________________________________
            }

            if (MovesList[gameIndex] < swapped)
            {
                MovesList[gameIndex] = swapped;
                //_____________________________________________________________________________________________
                //stores the swapped values in DataBase's moveslist at position gameIndex
                //_____________________________________________________________________________________________
            }

            StartCalander();
            coinAnim.AddCoins(0);
            
            DailyDataClass tempObject = new DailyDataClass();
            tempObject.scoreList = ScoreList.ToArray();
            tempObject.movesList = MovesList.ToArray();

            await ApiHelper.SendJSONData(postURL + userProfileURL + updateStatsURL, tempObject);


            //update rank if gameType == Daily
            if (gameIndex == DayNum)
            {
                if (score > ScoreList[gameIndex])
                {
                    var temp = await ApiHelper.RequestJSONData(getURL+userDailyDataURL);
                    GetDailyResponse UserDailyData = JsonUtility.FromJson<GetDailyResponse>(temp.response);
                    string diceleID = UserDailyData.info.dicele._id;
                    //_____________________________________________________________________________________________
                    //updates rank for the above score value
                    ScoreClass Object = new ScoreClass();
                    Object.score = score;
                    Object.diceleId = diceleID;
                    await ApiHelper.SendJSONData(postURL+userDailyDataURL, Object);
                    //_____________________________________________________________________________________________
                }
            }

            
        }
        else
        {
            ClassicLvlIndex++;
            StartClassicLvl();
            UIManager.UpdateClassicLevel(ClassicLvlIndex + 1);

            ClassicStreak++;
            if(BestClassicStreak < ClassicStreak) BestClassicStreak = ClassicStreak;

            int goldNum = 0;
            int winsAwayFromReward = 0;
            bool streakWon = false;
            for(int i = 0; i < piggyArr.Count(); i++)
            {   
                if(ClassicStreak < piggyArr[i])
                {
                    winsAwayFromReward = piggyArr[i] - ClassicStreak;
                    goldNum = ClassicStreak*10;
                    break;
                }

                if(piggyArr[i] == ClassicStreak)
                {
                    goldNum = piggyArr[i]*10;
                    winsAwayFromReward = piggyArr[i+1] - piggyArr[i];
                    streakWon = true;
                    break;
                }
            }

            UIManager.UpdatePlayerStats(PlayerXp,PlayerCoins);


            if(streakWon)
            {
                UIManager.OpenScreen("PiggyBankUnlocked_Canvas");
                UIManager.UpdatePiggyScreen(goldNum, winsAwayFromReward, 1);
                PlayerCoins += goldNum;
            }
            else
            {
                if(ClassicLvlIndex!=1 && ClassicLvlIndex != 2) {UIManager.OpenScreen("PiggyBankCongratulations_Canvas"); coinAnim.AddCoins(2);}
                UIManager.UpdatePiggyScreen(goldNum, winsAwayFromReward, 0);
            }

            
            

            //_____________________________________________________________________________________________
            //updates the currentClassicLvl variable for the given playerID
            ClassicIndexClass Object = new ClassicIndexClass();
            Object.classicIndex = ClassicLvlIndex;
            await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object);
            //_____________________________________________________________________________________________

            
            //________________________________________________________________________
            ClassicStreakClass Object2 = new ClassicStreakClass();
            Object2.ClassicStreak = ClassicStreak;
            Object2.BestClassicStreak = BestClassicStreak;
            await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object2);
            //________________________________________________________________________

        }

        //-----------------------------------------------------------------------
        //Update Player Coins and Xp

        CoinsClass Object0 = new CoinsClass();
        Object0.playerCoins = PlayerCoins;
        await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object0);

        XpClass Object1 = new XpClass();
        Object1.playerXp = PlayerXp;
        await ApiHelper.SendJSONData(postURL+userProfileURL+updateStatsURL, Object1);
        //-----------------------------------------------------------------------
    }


    public void playCoinAnim()
    {
        coinAnim.AddCoins(1);
        UIManager.UpdatePlayerStats(PlayerXp,PlayerCoins);

    }


    void OnDisable()
    {
        ActionEvents.StartLvl -= StartLvl;
        ActionEvents.TriggerGameWinEvent -= GameWinEvent;
        ActionEvents.TriggerGameLoseEvent -= GameLoseEvent;

    }
}
