using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DataManager : MonoBehaviour
{

    private List<int> solList = new List<int>();
    private List<int> spawnList = new List<int>();
    private int gameIndex;
    public int ClassicLvlIndex;
    private bool isDaily = true;

    DateTime StartDate = new DateTime(2023,1,10);
    
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
    private int PlayerXp;
    private int PlayerCoins;




    private UIManager UIManager;


    private int[] tempExampleDailySolArr = {5, 5, 2, 3, 1, 3, -1, 0, -1, 4, 4, 4, 3, 5, 0, 3, -1, 0, -1, 0, 1, 2, 1, 1, 2};
    private int[] tempExampleDailySpawnArr = {0, 2, 4, 1, 5, 2, -1, 5, -1, 0, 3, 3, 5, 0, 2, 0, -1, 1, -1, 1, 3, 1, 3, 4, 4};

    private int[] tempExampleClassicSolArr0 = {3,1,0,5};
    private int[] tempExampleClassicSpawnArr0 = {5,1,0,3};
    private int[] tempExampleClassicSolArr1 = {0,1,4,1,-1,0,5,0,2};
    private int[] tempExampleClassicSpawnArr1 = {2,1,0,1,-1,0,5,0,4};
    private int[] tempExampleClassicSolArr2 = {0,1,2,3,4,-1,5,4,3,2,-1,1,0,1,2,3};
    private int[] tempExampleClassicSpawnArr2 = {3,0,2,4,4,-1,2,3,3,2,-1,1,1,1,5,0};



    void OnEnable()
    {
        ActionEvents.StartLvl += StartLvl;
        ActionEvents.TriggerGameWinEvent += GameWinEvent;
        ActionEvents.TriggerGameLoseEvent += GameLoseEvent;
        ActionEvents.UpdateGameState += UpdateGameState;

        UIManager = GetComponent<UIManager>();
    }


    void Start()
    {

        DayNum = (DateTime.Now - StartDate).Days;
        MonthNum = (DateTime.Now.Month - StartDate.Month) + 12*(DateTime.Now.Year - StartDate.Year);


        ScoreList = new List<int>(DayNum + 1);
        MovesList = new List<int>(DayNum + 1);
        TrophyList = new List<int>(MonthNum);


        //_____________________________________________________________________________________________
        //retrieves the score,swap,trophy Lists from the database and stores them into the above Lists.
        //_____________________________________________________________________________________________

        //Setting all elements to zero for testing purpose
        ScoreList = Enumerable.Repeat(0, DayNum + 1).ToList();
        MovesList = Enumerable.Repeat(-2, DayNum + 1).ToList();
        TrophyList = Enumerable.Repeat(0, MonthNum).ToList();


        //_____________________________________________________________________________________________
        //retrieves the playerXp and playerCoins and ClassicLvlIndex from the database and stores them into the given variables.
        //_____________________________________________________________________________________________
        ClassicLvlIndex = 0;
        PlayerXp = 0;
        PlayerCoins = 0;
        UIManager.UpdateMainScreenStats(PlayerXp,PlayerCoins);
        UIManager.UpdateClassicLevel(ClassicLvlIndex + 1);


        //get daily and weekly rank from leaderboard

        //get stats 
    

    }



    /// <summary>
    /// This function is called when user clicks on the daily dicele button.
    /// </summary>

    public void StartCalander()
    {
        if(isDaily)
        {
            UIManager.OpenScreen("Calander_Canvas");
            ActionEvents.StartCalander(MovesList);
        }
        else{
            isDaily = true;
            UIManager.OpenScreen("Main_Menu_Canvas");
        }
    }



    /// <summary>
    /// This function is called when user clicks on a calander button to start a new game.
    /// </summary>

    private void StartLvl(int levelNum)
    {
        isDaily = true;
        gameIndex = levelNum;
        //________________________________________________________________________________________________
        //fetches nth(gameIndex) array of Solution and Spawn GameData fields from database and stores in solList and spawnList
        //________________________________________________________________________________________________
        StoreGameData(tempExampleDailySpawnArr,tempExampleDailySolArr);

        currStateList = new List<int>(spawnList);

        UIManager.UpdateGameScreen(PlayerCoins, StartDate.AddDays(gameIndex).Day, StartDate.AddDays(gameIndex).ToString("MMMM").Substring(0,3) , 100 , 50);


        //Opens the game canvas
        UIManager.OpenScreen("GamePlay_Canvas");
        UIManager.CloseScreen("Calander_Canvas");


        //Sends the Lists to gameBoard
        ActionEvents.SendGameData(solList,spawnList,0);


    }


    public void StartClassicLvl()
    {
        isDaily = false;   
        //________________________________________________________________________________________________
        //fetches nth(ClassicLvlIndex) array of Solution and Spawn GameData fields from database and stores in solList and spawnList
        //________________________________________________________________________________________________
        if(ClassicLvlIndex == 0)    StoreGameData(tempExampleClassicSpawnArr0,tempExampleClassicSolArr0);
        else if(ClassicLvlIndex == 1)   StoreGameData(tempExampleClassicSpawnArr1,tempExampleClassicSolArr1);
        else    StoreGameData(tempExampleClassicSpawnArr2,tempExampleClassicSolArr2);

        currStateList = new List<int>(spawnList);

        //Opens the game canvas
        UIManager.OpenScreen("GamePlay_Canvas");


        //Sends the Lists to gameBoard
        ActionEvents.SendGameData(solList,spawnList,0);


    }

    private void StoreGameData(int[] spawn, int[] sol)
    {
        solList.Clear();
        spawnList.Clear();

        for(int i = 0; i < spawn.Count(); i ++)
        {
            solList.Add(sol[i]);
            spawnList.Add(spawn[i]);
        }
    }



    /// <summary>
    /// This function is called when user selects the option Add 3 moves
    /// </summary>

    private void StartCustomLevel()
    {

        //________________________________________________________________________________________________
        //fetches nth(gameIndex) array of Solution GameData fields from database and stores in solList
        //________________________________________________________________________________________________
        StoreGameData(tempExampleDailySolArr,currStateList.ToArray());

        //Sends the Lists to gameBoard 
        ActionEvents.SendGameData(solList,spawnList,1);

        //Opens the game canvas
        UIManager.OpenScreen("GamePlay_Canvas");
        UIManager.CloseScreen("Calander_Canvas");
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
        int score = ((playtime < 50) ? 500 : (playtime > 290 ? 20 : (300 - playtime)*2)) + (21 - swapped)*100 + 1000; 
        int xp = 0;
        int coins;
        int rank = 0;

        if(isDaily)
        {
            coins = (xp - 10 > 30 ? (xp - 10)/100 : 30);
            // Winning Bonus(10) + ScoreImprovementBonus
            xp = 10 + ((score - ScoreList[gameIndex]) > 0 ? (score - ScoreList[gameIndex]) : 0) /100;
        }
        else
        {
            coins = 10;
            xp = 10 + (score)/100;
        }


        //Updating Database
        if(isDaily)
        {
            
            if(ScoreList[gameIndex] < score)
            {
                //_____________________________________________________________________________________________
                //stores the score value in DataBase's scorelist at position gameIndex
                //_____________________________________________________________________________________________
            }

            if(MovesList[gameIndex] > swapped)
            {
                //_____________________________________________________________________________________________
                //stores the swapped values in DataBase's moveslist at position gameIndex
                //_____________________________________________________________________________________________
            }


            //get rank if gameType == Daily
            if( gameIndex  == DayNum)
            {
                if(score > ScoreList[gameIndex])
                {
                    //_____________________________________________________________________________________________
                    //updates rank for the above score value
                    //_____________________________________________________________________________________________
                }

                //_____________________________________________________________________________________________
                //fetches rank for the above score value
                //_____________________________________________________________________________________________

                rank = 1000;
            }
        }
        else
        {
            ClassicLvlIndex ++;
            //_____________________________________________________________________________________________
            //updates the currentClassicLvl variable for the given playerID
            //_____________________________________________________________________________________________
        }


        //Show Results
        if(gameIndex == DayNum && isDaily){
            UIManager.UpdateResultScreen(score,rank,xp,coins,true);
        }
        else{
            UIManager.UpdateResultScreen(score,rank,xp,coins,false);
        }

        if(isDaily || (ClassicLvlIndex!=1 && ClassicLvlIndex!=2)){
            UIManager.OpenScreen("Level_Complete_Canvas");
            UIManager.CloseScreen("GamePlay_Canvas");
        }
        else{
            //trigger event for tutorial levels
        }

    }


    private void GameLoseEvent()
    {
        UIManager.OpenScreen("Level_Failed_Canvas");
        UIManager.CloseScreen("GamePlay_Canvas");
    }

    public void RetryLvl(bool RetryAfterWin)
    {
        if(RetryAfterWin)
        {
            StartLvl(gameIndex);   
        }
        else{
            if(isDaily)
            {

            }
            else{
                
            }
        }
    }


    void OnDisable()
    {
        ActionEvents.StartLvl -= StartLvl;
        ActionEvents.TriggerGameWinEvent -= GameWinEvent;
        ActionEvents.TriggerGameLoseEvent -= GameLoseEvent;

    }
}
