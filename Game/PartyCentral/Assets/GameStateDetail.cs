using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using partycentral;

public class GameStateDetail : MonoBehaviour
{
    public GameStages CurrentStage;
    public int DaysLeft;
    public int MoneyEarned;
    public string FriendName;
    public int TimeClock;  // stored as 15 minute increments, starting at 10PM and ending at 8PM
    public bool LightsOn;

    private int PeriodFraction;

    public string getTimeStr()
    {
        int hour = 10;
        hour = hour + (TimeClock / 4);
        int min = (TimeClock % 4) * 15;
        if (hour > 12) {
            hour = hour - 12;
        }
        string printable = $"{hour}:{min:D2}";
        return printable;
    }

    public bool isMorning() 
    {
        if (TimeClock >= ((2 + 8) * 4)) // 8AM
        { 
            return true;
        }
        return false;
    }

    public bool isLate() {
        if (LightsOn) {
            return true;
        }
        if (TimeClock  >= (2 * 4)) { // 12AM
            return true;
        }
        return false;
    }

    public bool isVeryLate() {
        if (TimeClock >= ((2 + 4) * 4)) { // 4AM
            return true;
        }
        return false;
    }

    public string getMStr()
    {
        int hour = 10;
        hour += (TimeClock / 4);
        int min = (TimeClock % 4) * 15;
        if (hour > 12) {
            hour -= 12;
        };
        string printable = $"{hour}:{min:D2}";
        return printable;
    }

    void Tick() {
        TimeClock += 1;
    }

    void OverallStageHandler() {
        switch (CurrentStage) {
            case GameStages.PartyPlay:
                if (PeriodFraction == 0) Tick();
                if (isMorning()) CurrentStage = GameStages.ParentHome;
                break;
            case GameStages.ParentHome:
                break;
            default:
              break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DaysLeft = 20;
        MoneyEarned = 50;
        FriendName = "TBD";
        TimeClock = 0;
        LightsOn = false;
        PeriodFraction = 0;
        CurrentStage = GameStages.PartyPlay;
    }

    // Update is called once per frame
    void Update()
    {
        PeriodFraction += 1;
        if (PeriodFraction > 120) {  // 120 = 4 seconds
            PeriodFraction = 0;
            string status = getTimeStr() + "  " + Enum.GetName(typeof(GameStages), CurrentStage);
            Debug.Log(status);            
        }
        OverallStageHandler();
    }
}
