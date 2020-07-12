using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using partycentral;

public class GameStateDetail : MonoBehaviour
{
    public GameStages currentStage;
    public int DaysLeft;
    public int MoneyEarned;
    public string FriendName;
    public int Time;  // stored as 15 minute increments, starting at 10PM and ending at 8PM
    public bool LightsOn;

    public string getTimeStr()
    {
        int hour = 10;
        hour = hour + (Time / 4);
        int min = (Time % 4) * 15;
        if (hour > 12) {
            hour = hour - 12;
        }
        string printable = $"{hour}:{min:D2}";
        return printable;
    }

    public bool isMorning() 
    {
        if (Time >= ((2 + 8) * 4)) // 8AM
        { 
            return true;
        }
        return false;
    }

    public bool isLate() {
        if (LightsOn) {
            return true;
        }
        if (Time  >= (2 * 4)) { // 12AM
            return true;
        }
        return false;
    }

    public bool isVeryLate() {
        if (Time >= ((2 + 4) * 4)) { // 4AM
            return true;
        }
        return false;
    }

    public string getMStr()
    {
        int hour = 10;
        hour += (Time / 4);
        int min = (Time % 4) * 15;
        if (hour > 12) {
            hour -= 12;
        };
        string printable = $"{hour}:{min:D2}";
        return printable;
    }

    // Start is called before the first frame update
    void Start()
    {
        DaysLeft = 20;
        MoneyEarned = 50;
        FriendName = "TBD";
        Time = 0;
        LightsOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        Time = Time + 1;        
    }
}
