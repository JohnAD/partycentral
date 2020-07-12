using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace partycentral {

    public enum GameStages {
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

    // public string[] LoveLevelMsgs = {
    //     "Break up time.",
    //     "Eh. At least you aren't gross.",
    //     "You are okay.",
    //     "I “Like” you.",
    //     "My heart flutters.",
    //     "I'm looking forward to more.",
    //     "I'm willing to use the “L” word.",
    //     "Wowsa!",
    //     "I'm head over heels.",
    //     "One true love.",
    // };

    public enum CharacterRole {
        ThePlayer,
        LoveInterest,
        Dad,
        Mom,
        Bartender,
        RandomGuest,
    };

    public enum Desire {
        Nothing,
        Drink,
        Dance,
        Talk,
        Leave
    };

    public enum Action {
        Idle,
        Conversation,
        Seeking,
        Drinking,
        MixingDrink,
        Serving,
        Dancing,
        Leaving,
        Screaming,
        Flirting,             // only done by ThePlayer, so no animation
        Cleaning,             // only done by ThePlayer, so no animation
    };

    public enum AnimationStyle {
        Idle,             // for Idle
        MovingSober,      // for Seeking, Wander, Leaving
        MovingDrunk,      // for Seeking, Wander, Leaving (when InebriationLevel > 3)
        Drinking,         // for Drinking
        Mixing,           // for Mixing
        Serving,          // for Serving
        Dancing,          // for Dancing
        Anger,            // for Screaming
        Running,          // for Leaving (after 8am)
    };

    public class CharacterBehavior
    {

        public const float NormalDecisionDuration = 2f;

        public CharacterRole Role;
        public int ConversationTarget;
        public Action MyAction;
        public Desire MyDesire;
        public AnimationStyle MyAnimation;
        public int InebriationLevel;
        public int SeekTarget;
        public int MovementSpeed;

        private float LastDecisionDuration;
        private float NearDistance = 10f;     // TODO get real values
        private float OkDistance = 20f;       // TODO get real values
        private float AdjacentDistance = 1f;  // TODO get real values

        private string OwnName;

        public CharacterBehavior(CharacterRole role, string ownName)
        {
            Role = role;
            OwnName = ownName;
            MyAction = Action.Idle;
            MyDesire = Desire.Nothing;
            MyAnimation = AnimationStyle.Idle;
            InebriationLevel = 0; 
            LastDecisionDuration = float.PositiveInfinity;
            MovementSpeed = 0;
        }

        private float getDistanceToNearest(GameObject self, string tag) {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

            float minDist = float.PositiveInfinity;

            foreach (GameObject target in targets)
            {
                float dist = Vector3.Distance(self.transform.position, target.transform.position);
                if (dist < minDist) minDist = dist;
            }

            return minDist;
        }

        public void UpdateDecision() { // called once (and only once) per frame update
            // for the moment, we assume an endless party
            GameObject self = GameObject.Find(OwnName);
            GameObject gso = GameObject.Find("GameStateGlob"); //
            GameStateDetail gs = gso.GetComponent<GameStateDetail>();

            float distanceToNearestGuest = getDistanceToNearest(self, "RandomGuest");
            float distanceToNearestBar = getDistanceToNearest(self, "Bar");
            float distanceToNearestMusic = getDistanceToNearest(self, "BoomBox");
            float distanceToFrontDoor = getDistanceToNearest(self, "FrontDoor");;

            if (LastDecisionDuration >= NormalDecisionDuration) {
                //
                // first handle desire
                //
                if (MyDesire == Desire.Nothing)
                {
                    List<Desire> possibleDesires;
                    LastDecisionDuration = 0f;
                    switch (Role)
                    {
                        case CharacterRole.RandomGuest:
                        case CharacterRole.LoveInterest:
                            if (gs.isMorning()) {
                                possibleDesires = new List<Desire>() {Desire.Leave};
                            } else {
                                possibleDesires = new List<Desire>() {Desire.Drink, Desire.Dance, Desire.Talk};
                                if (distanceToNearestGuest < NearDistance) possibleDesires.Add(Desire.Talk);
                                if (distanceToNearestGuest < OkDistance) possibleDesires.Add(Desire.Talk);
                                if (distanceToNearestBar < NearDistance) possibleDesires.Add(Desire.Drink);
                                if (distanceToNearestBar < OkDistance) possibleDesires.Add(Desire.Drink);
                                if (distanceToNearestMusic < NearDistance) possibleDesires.Add(Desire.Dance);
                                if (distanceToNearestMusic < OkDistance) possibleDesires.Add(Desire.Dance);                              
                                if (gs.isLate()) possibleDesires.Add(Desire.Leave);
                                if (gs.isLate()) possibleDesires.Add(Desire.Leave);
                                if (gs.isVeryLate()) possibleDesires.Add(Desire.Leave);
                                if (gs.isVeryLate()) possibleDesires.Add(Desire.Leave);
                                if (gs.isVeryLate()) possibleDesires.Add(Desire.Leave);
                                if (gs.isVeryLate()) possibleDesires.Add(Desire.Leave);
                            }
                            break;
                        default:
                            possibleDesires = new List<Desire>() {Desire.Nothing};
                            break;
                    };
                    int desireIndex = Random.Range(0, possibleDesires.Count);
                    MyDesire = possibleDesires[desireIndex];
                };
                //
                // change action based on desire
                //
                switch (MyDesire)
                {
                    case Desire.Talk:
                        if (distanceToNearestGuest <= AdjacentDistance) {
                            MyAction = Action.Conversation;
                            MyDesire = Desire.Nothing;
                        } else {
                            MyAction = Action.Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    case Desire.Dance:
                        if (distanceToNearestBar <= NearDistance) {
                            MyAction = Action.Dancing;
                            MyDesire = Desire.Nothing;
                        } else {
                            MyAction = Action.Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    case Desire.Drink:
                        if (distanceToNearestBar <= AdjacentDistance) {
                            MyAction = Action.Drinking;
                            MyDesire = Desire.Nothing;
                            // TODO: handle drink purchase
                        } else {
                            MyAction = Action.Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    case Desire.Leave:
                        if (distanceToFrontDoor <= AdjacentDistance) {
                            // TODO: handle "exit" of guest
                            MyAction = Action.Idle;
                            // we do NOT reset desire once the desire to leave starts
                        } else {
                            MyAction = Action.Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    default:
                        MyAction = Action.Idle;
                        MyDesire = Desire.Nothing;
                        break;
                }
                //
                // change animation based on action
                //
                switch (MyAction) {
                    case Action.Conversation:
                        MyAnimation = AnimationStyle.Idle;
                        MovementSpeed = 0;
                        break;
                    case Action.Seeking:
                        if (InebriationLevel >= 3) {
                            MyAnimation = AnimationStyle.MovingDrunk;
                            MovementSpeed = 2;
                        } else {
                            MyAnimation = AnimationStyle.MovingSober;
                            MovementSpeed = 3;
                        }
                        break;
                    case Action.Drinking:
                        MyAnimation = AnimationStyle.Drinking;
                        MovementSpeed = 0;
                        break;
                    case Action.MixingDrink: // only done by bartender (if we make one)
                        MyAnimation = AnimationStyle.Mixing;
                        MovementSpeed = 0;
                        break;
                    case Action.Serving: // only done by bartender (if we make one)
                        MyAnimation = AnimationStyle.Serving;
                        MovementSpeed = 0;
                        break;
                    case Action.Dancing:
                        MyAnimation = AnimationStyle.Dancing;
                        MovementSpeed = 0;
                        break;
                    case Action.Leaving:
                        if (gs.isMorning()) {
                            MyAnimation = AnimationStyle.Running;
                            MovementSpeed = 7;
                        } else {
                            if (InebriationLevel >= 3) {
                                MyAnimation = AnimationStyle.MovingDrunk;
                                MovementSpeed = 2;
                            } else {
                                MyAnimation = AnimationStyle.MovingSober;
                                MovementSpeed = 3;
                            }
                        }
                        break;
                    case Action.Screaming:
                        MyAnimation = AnimationStyle.Anger;
                        MovementSpeed = 0;
                        break;
                    case Action.Flirting: // Only done by player
                    case Action.Cleaning: // Only done by player
                        MyAnimation = AnimationStyle.Idle; 
                        MovementSpeed = 0;
                        break;
                    default:
                        break;
                }
            };
            //
            //
            LastDecisionDuration += Time.deltaTime;
        }
    }
    
    // public class IdleStateBehavior : StateMachineBehavior 
    // {
    //     private float lastPoseChangeDuration = float.PositiveInfinity;
    //     private float poseChangeFrequency = 2f;
    //     private float startPose;
    //     private float endPose;
    //     private int poseParamHash;

    //     public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //         poseParamHash = Animator.StringToHash("Pose");
    //         base.OnStateEnter(animator, stateInfo, layerIndex);
    //     }

    //     public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //         lastPoseChangeDuration += Time.deltaTime;
    //         if (lastPoseChangeDuration >= poseChangeFrequency) {
    //             startPose = animator.GetFloat("Pose");
    //             endPose = Random.Range(0f, 1f);
    //             lastPoseChangeDuration = 0;
    //         }
    //         animator.setFloat(postParmHash, Mathf.Lerp(startPose, endPose, lastPoseChangeDuration / poseChangeFrequency));
    //         base.OnStateUpdate(animator, stateInfo, layerIndex);
    //     }
    // }

    // public class MovingSoberStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // }

    // public class MovingDrunkStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // };

    // public class DrinkingStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // };

    // public class MixingStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // };

    // public class ServingStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // };

    // public class DancingStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // };

    // public class AngerStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // };

    // public class RunningStateBehavior : StateMachineBehavior 
    // {
    //     x;
    // };

}
