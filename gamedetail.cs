namespace partycentral {
    
    public class GameDetail
    {

        public GameStages currentStage;
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

};

