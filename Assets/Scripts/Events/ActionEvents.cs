using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ActionEvents
{
    public static Action<int> StartLvl;
    public static Action<List<int>,List<int>,int> SendGameData;
    public static Action<int,int> UpdateGameState;
    public static Action<Dice> swapDice;
    public static Action TriggerGameLoseEvent;
    public static Action<int,int> TriggerGameWinEvent;
    public static Action<List<int>> StartCalander;
}
