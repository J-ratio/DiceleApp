using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ActionEvents
{
    public static Action<int> StartLvl;
    public static Action<List<int>,List<int>,int,bool> SendGameData;
    public static Action<int,int> UpdateGameState;
    public static Action<Dice> swapDice;
    public static Action TriggerGameLoseEvent;
    public static Action<int,int> TriggerGameWinEvent;
    public static Action<List<int>> StartCalander;
    public static Action Undo;
    public static Action Hint;
    public static Action<List<int>> SendTrophyList;
}
