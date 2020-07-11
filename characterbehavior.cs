namespace partycentral {

	enum CharacterRole {
		ThePlayer,
		LoveInterest,
		Dad,
		Mom,
		Bartender,
		RandomGuest,
	};

	enum CurrentAction {
		Idle,
		Conversation,
		Seeking,
		Drinking,
		MixingDrink,
		Serving,
		Dancing,
	    Wander,
	    Leaving,
	    Screaming,
	    Flirting,             // only done by ThePlayer, so no animation
	    Cleaning,             // only done by ThePlayer, so no animation
	};

	enum CurrentAnimation {
		AnimIdle,             // for Idle
		AnimMovingSober,      // for Seeking, Wander, Leaving
		AnimMovingDrunk,      // for Seeking, Wander, Leaving (when InebriationLevel > 3)
		AnimDrinking,         // for Drinking
		AnimMixing,           // for Mixing
		AnimServing           // for Serving
		AnimDancing,          // for Dancing
		AnimAnger,            // for Screaming
		AnimRunning,          // for Leaving (after 8am)
	}

	public class CharacterBehavior
	{

	    public CharacterRole Role;
	    public int ConversationTarget;
	    public CurrentAction Action;
	    public int InebriationLevel;
	    public int SeekTarget;

		public CharacterBehavior(CharacterRole role)
	    {
	        Role = role;
	        Action = Idle;
	    }

	}
	
	public class IdleStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class MovingSoberStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class MovingDrunkStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class DrinkingStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class MixingStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class ServingStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class DancingStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class AngerStateBehavior : StateMachineBehavior 
	{
		x;
	};

	public class RunningStateBehavior : StateMachineBehavior 
	{
		x;
	};

}
