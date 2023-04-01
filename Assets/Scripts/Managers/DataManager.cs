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

    DateTime StartDate = new DateTime(2023, 1, 10);

    // # of days since start date
    private int DayNum;

    // # of month since start date
    private int MonthNum;


    //stores the board dice values of the current ongoing game 
    //later to be used as a fallback feature in case player disconnects
    private List<int> currStateList;


    //Database values to be retrieved 
    private List<int> ScoreList;
    private List<int> MovesList;
    private List<int> TrophyList;
    private int PlayerXp = 0;
    private int PlayerCoins = 0;
    int[] queuedWinCase = new int[4];




    private UIManager UIManager;
    public CoinsManager coinAnim;


    private int[] tempExampleDailySolArr = { 5, 5, 2, 3, 1, 3, -1, 0, -1, 4, 4, 4, 3, 5, 0, 3, -1, 0, -1, 0, 1, 2, 1, 1, 2 };
    private int[] tempExampleDailySpawnArr = { 0, 2, 4, 1, 5, 2, -1, 5, -1, 0, 3, 3, 5, 0, 2, 0, -1, 1, -1, 1, 3, 1, 3, 4, 4 };
    private int[] tempExampleClassicSolArr0 = { 3, 1, 0, 5 };
    private int[] tempExampleClassicSpawnArr0 = { 5, 1, 0, 3 };
    private int[] tempExampleClassicSolArr1 = { 0, 1, 4, 1, -1, 0, 5, 0, 2 };
    private int[] tempExampleClassicSpawnArr1 = { 2, 1, 0, 1, -1, 0, 5, 0, 4 };
    private int[] tempExampleClassicSolArr2 = { 0, 1, 2, 3, 4, -1, 5, 4, 3, 2, -1, 1, 0, 1, 2, 3 };
    private int[] tempExampleClassicSpawnArr2 = { 3, 0, 2, 4, 4, -1, 2, 3, 3, 2, -1, 1, 1, 1, 5, 0 };

    [SerializeField] private GamePlayTutorial playTutorial;//Added by charan

    void OnEnable()
    {
        ActionEvents.StartLvl += StartLvl;
        ActionEvents.TriggerGameWinEvent += GameWinEvent;
        ActionEvents.TriggerGameLoseEvent += GameLoseEvent;
        ActionEvents.UpdateGameState += UpdateGameState;

        UIManager = GetComponent<UIManager>();
    }


    [Serializable]
    public class MyClass
    {
        public int level;
        public float timeElapsed;
        public string playerName;
    }

    [Serializable]
    public class DataObjectInGetResponse
    {
        public string id;
        public string email;
        public string first_name;
        public string last_name;
        public string avatar;
    }

    [Serializable]
    public class SupportObjectInGetResponse
    {
        public string url;
        public string text;
    }

    [Serializable]
    public class GetResponse {
        public DataObjectInGetResponse data;
        public SupportObjectInGetResponse support;
    }
    async void Start()
    {
        var postUrl = "https://typedwebhook.tools/webhook/f88dcdc3-19c8-40ff-a2fd-099a228ac8c8";
        MyClass myObject = new MyClass();
        myObject.level = 1;
        myObject.timeElapsed = 47.5f;
        myObject.playerName = "Dr Charles Francis";

        var UserData = await ApiHelper.SendJSONData(postUrl, myObject);
        Debug.Log(UserData.response);

        var getUrl = "https://reqres.in/api/users/2";
        var GetData = await ApiHelper.RequestJSONData(getUrl);
        Debug.Log(GetData.response);
        
        GetResponse r = JsonUtility.FromJson<GetResponse>(GetData.response);
        Debug.Log(JsonUtility.ToJson(r.data));

        DayNum = (DateTime.Now - StartDate).Days;
        MonthNum = (DateTime.Now.Month - StartDate.Month) + 12 * (DateTime.Now.Year - StartDate.Year);


        ScoreList = new List<int>(DayNum + 1);
        MovesList = new List<int>(DayNum + 1);
        TrophyList = new List<int>(MonthNum);


        //_____________________________________________________________________________________________
        //retrieves the score,swap from the database and stores them into the above Lists.
        //_____________________________________________________________________________________________

        //Setting all elements to zero for testing purpose
        ScoreList = Enumerable.Repeat(0, DayNum + 1).ToList();
        MovesList = Enumerable.Repeat(-2, DayNum + 1).ToList();

        //TrophyList = MakeTrophyList(MovesList);


        //_____________________________________________________________________________________________
        //retrieves the playerXp and playerCoins and ClassicLvlIndex from the database and stores them into the given variables.
        //_____________________________________________________________________________________________
        ClassicLvlIndex = 0;
        PlayerXp = 0;
        PlayerCoins = 5500;
        UIManager.UpdatePlayerStats(PlayerXp, PlayerCoins);
        UIManager.UpdateClassicLevel(ClassicLvlIndex + 1);


        //get daily and weekly rank from leaderboard

        //get stats 

    }



    /// <summary>
    /// This function is called when user clicks on the daily dicele button.
    /// </summary>

    public void StartCalander()
    {
        if (isDaily)
        {
            UIManager.OpenScreen("Calander_Canvas");
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

    private void StartLvl(int levelNum)
    {
        playTutorial.isClassic = false;//Added by charan
        isDaily = true;
        gameIndex = levelNum;
        currentUndoNum = 0;
        currentHintNum = 0;
        //________________________________________________________________________________________________
        //fetches nth(gameIndex) array of Solution and Spawn GameData fields from database and stores in solList and spawnList
        //________________________________________________________________________________________________
        StoreGameData(tempExampleDailySpawnArr, tempExampleDailySolArr);

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
        else StoreGameData(tempExampleClassicSpawnArr2, tempExampleClassicSolArr2);

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

    private void MakeTrophyList(List<int> movesList)
    {

    }


    public void Undo()
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
            //-----------------------------------------------------------------------
            //Update Player Coins
            //-----------------------------------------------------------------------
            UIManager.GameScreenUI[3].text = CoinOptions[currentUndoNum > 1 ? 2 : currentUndoNum].ToString();
            ActionEvents.Undo();
        }

    }



    public void Hint()
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
            //-----------------------------------------------------------------------
            //Update Player Coins
            //-----------------------------------------------------------------------
            UIManager.GameScreenUI[4].text = CoinOptions[currentHintNum > 2 ? 3 : currentHintNum].ToString();

            ActionEvents.Hint();
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
        //________________________________________________________________________________________________
        StoreGameData(tempExampleDailySolArr, currStateList.ToArray());

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


    private void GameWinEvent(int swapped, int playtime)
    {

        // TimeBonus + MoveBonus + WinningBonus(1000)
        int score = ((playtime < 50) ? 500 : (playtime > 290 ? 20 : (300 - playtime) * 2)) + (21 - swapped) * 100 + 1000;
        int xp = 0;
        int coins;
        int rank = 0;



        //_____________________________________________________________________________________________
        //fetches the rank for the above score value
        //_____________________________________________________________________________________________

        rank = 1000;


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
            UIManager.UpdateResultScreen(score, rank, xp, coins, true);
        }
        else
        {
            UIManager.UpdateResultScreen(score, rank, xp, coins, false);
        }

        if (isDaily || (ClassicLvlIndex != 0 && ClassicLvlIndex != 1))
        {
            UIManager.OpenScreen("Level_Complete_Canvas");
        }
        else
        {
            UIManager.OpenScreen("Level_TutComplete_Canvas"); //Added by charan
        }

        UIManager.CloseScreen("GamePlay_Canvas");
    }

    private void GameLoseEvent()
    {


        //_____________________________________________________________________________________________
        //stores -1 in DataBase's moveslist at position gameIndex
        //_____________________________________________________________________________________________
        if (isDaily)
        {
            MovesList[gameIndex] = -1;
        }

        UIManager.OpenScreen("Level_Failed_Canvas");
        UIManager.CloseScreen("GamePlay_Canvas");
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


    public void Collect()
    {

        int score = queuedWinCase[3];
        int swapped = queuedWinCase[2];

        //Updating Database
        if (isDaily)
        {
            StartCalander();

            if (ScoreList[gameIndex] < score)
            {
                ScoreList[gameIndex] = score;
                //_____________________________________________________________________________________________
                //stores the score value in DataBase's scorelist at position gameIndex
                //_____________________________________________________________________________________________
            }

            if (MovesList[gameIndex] > swapped)
            {
                MovesList[gameIndex] = swapped;
                //_____________________________________________________________________________________________
                //stores the swapped values in DataBase's moveslist at position gameIndex
                //_____________________________________________________________________________________________
            }


            //update rank if gameType == Daily
            if (gameIndex == DayNum)
            {
                if (score > ScoreList[gameIndex])
                {
                    //_____________________________________________________________________________________________
                    //updates rank for the above score value
                    //_____________________________________________________________________________________________
                }
            }
        }
        else
        {
            UIManager.OpenScreen("Main_Menu_Canvas");
            ClassicLvlIndex++;
            UIManager.UpdateClassicLevel(ClassicLvlIndex + 1);
            //_____________________________________________________________________________________________
            //updates the currentClassicLvl variable for the given playerID
            //_____________________________________________________________________________________________
        }


        PlayerCoins = PlayerCoins + queuedWinCase[0];
        PlayerXp = PlayerXp + queuedWinCase[1];
        UIManager.UpdatePlayerStats(PlayerXp, PlayerCoins);

        //-----------------------------------------------------------------------
        //Update Player Coins and Xp
        //-----------------------------------------------------------------------

        UIManager.CloseScreen("Level_Complete_Canvas");
        UIManager.CloseScreen("Level_Failed_Canvas");


        if (isDaily || (ClassicLvlIndex != 1 && ClassicLvlIndex != 2))
        {
            coinAnim.AddCoins();
        }
    }


    void OnDisable()
    {
        ActionEvents.StartLvl -= StartLvl;
        ActionEvents.TriggerGameWinEvent -= GameWinEvent;
        ActionEvents.TriggerGameLoseEvent -= GameLoseEvent;

    }
}
