enum GameStages {
    ChooseLove,      // Choose the character to play as the love interest
	DayStart,        // Show status and days remaining.
    PartyChoose,     // Choose party conditions (Rent equipment, invitation count, drink price, etc.)
	PartyPlay,       // Basic party. Continues until “lights on”. Can flirt or clean during party. 
					 // When flirting, you can also add to the mess but your status with gfriend rises.
					 // When light’s on, goes to NIGHT_PARTY_CLEANUP. If reaches 8AM, goes directly to
					 // PARENTS_HOME. Sidebar shows girlfriend names and status.
    PartyCleanup,    // House lit up. Guests start to leave. When reaches 8AM or player clicks “go to sleep”,
                     // goes to PARENTS_HOME. Shows “House is a mess. Better clean up fast.” when any trash is still around.
    ParentHome,      // One parent walks in front door. One parent (which may be off-screen) walks to first
                     // trash. Front door parent says “Good Morning!”. If no trash, does to END, otherwise PARENTS_HOME2.
    ParentScream,    // If any trash, then screen swings to screaming parent. Player notified of random
                     // number of party days lost.Goes to END.
    EndGame          // If gfriend status < 0, then the game is over and you lose. If money > $10K, and
                     // girlfriend status > N, then win. If out of days, then you lose.
};

string[] LoveLevelMsgs = {
	"Break up time.",
    "Eh. At least you aren't gross.",
    "You are okay.",
    "I “Like” you.",
    "My heart flutters.",
    "I'm looking forward to more.",
    "I'm willing to use the “L” word.",
    "Wowsa!",
    "I'm head over heels.",
    "One true love.",
}

public class GameDetail
{

    public int DaysLeft;
    public int MoneyEarned;
    public string FriendName;
    public int Time;  // stored as 15 minute increments, starting at 10PM and ending at 8PM

	public GameDetail()
    {
    	DaysLeft = 20;
    	MoneyEarned = 50;
    	FriendName = "TBD";
    	Time = 0;
    }

    public string getTimeStr()
    {
    	int hour = 10;
    	hour = hour + (Time / 4);
    	int min = (Time % 4) * 15;
    	if (hour > 12) {
    		hour = hour - 12;
    	}
    	string printable = $"{hour}:{min:D2}"
    	return 
    }

    public tick()
    {
    	Time = Time + 1;
    }

    public bool isMorning() {
    	if ((Time * 4) >= (2 + 8)) {
    		return true;
    	}
    	return false;
    }

    public string getMStr()
    {
    	int hour = 10;
    	hour = hour + (Time / 4);
    	int min = (Time % 4) * 15;
    	if (hour > 12) {
    		hour = hour - 12;
    	}
    	string printable = $"{hour}:{min:D2}"
    	return 
    }
}