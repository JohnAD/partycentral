using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace partycentral {

    enum CharacterRole {
        ThePlayer,
        LoveInterest,
        Dad,
        Mom,
        Bartender,
        RandomGuest,
    };

    enum CurrentDesire {
        DesireNothing,
        DesireDrink,
        DesireDance,
        DesireTalk,
        DesireLeave
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
    };

    public class CharacterBehavior
    {

        public CharacterRole Role;
        public int ConversationTarget;
        public CurrentAction Action;
        public CurrentDesire Desire;
        public CurrentAnimation Animation;
        public int InebriationLevel;
        public int SeekTarget;
        public int MovementSpeed;

        private float lastDecisionDuration;
        private float NearDistance = 10f;     // TODO get real values
        private float OkDistance = 20f;       // TODO get real values
        private float AdjacentDistance = 1f;  // TODO get real values

        private string OwnName;

        public CharacterBehavior(CharacterRole role, string ownName)
        {
            Role = role;
            OwnName = ownName;
            Action = Idle;
            Desire = DesireNothing;
            CurrentAnimation = AnimIdle;
            InebriationLevel = 0; 
            LastDecisionDuration = float.PositiveInfinity;
            MovementSpeed = 0;
        }

        private float getDistanceToNearest(GameObject self, string tag) {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

            float minDist = float.PositiveInfinity;

            foreach (GameObject target in targets)
            {
                float dist = Vector3.distance(self.position, target.position);
                if (dist < minDist) minDist = dist;
            }

            return minDist;
        }

        public UpdateDecision() { // called once (and only once) per frame update
            // for the moment, we assume an endless party
            GameObject self = GameObject.Find(OwnName);
            GameObject gs = GameObject.Find("GameState");
            float distanceToNearestGuest = getDistanceToNearest(self, "RandomGuest");
            float distanceToNearestBar = getDistanceToNearest(self, "Bar");
            float distanceToNearestMusic = getDistanceToNearest(self, "BoomBox");
            float distanceToFrontDoor = getDistanceToNearest(self, "FrontDoor");;

            if (LastDecisionDuration >= NormalDecisionDuration) {
                //
                // first handle desire
                //
                if (Desire == DesireNothing)
                {
                    CurrentDesire[] possibleDesires = {};
                    LastDecisionDuration = 0f;
                    switch (Role)
                    {
                        case RandomGuest:
                        case LoveInterest:
                            if (gs.isMorning()) {
                                possibleDesires = {DesireLeave};
                            } else {
                                possibleDesires = {DesireDrink, DesireDance, DesireTalk};
                                if (distanceToNearestGuest < NearDistance) possibleDesires.Add(DesireTalk);
                                if (distanceToNearestGuest < OkDistance) possibleDesires.Add(DesireTalk);
                                if (distanceToNearestBar < NearDistance) possibleDesires.Add(DesireDrink);
                                if (distanceToNearestBar < OkDistance) possibleDesires.Add(DesireDrink);
                                if (distanceToNearestMusic < NearDistance) possibleDesires.Add(DesireDance);
                                if (distanceToNearestMusic < OkDistance) possibleDesires.Add(DesireDance);                              
                                if (gs.isLate()) possibleDesires.Add(DesireLeave);
                                if (gs.isLate()) possibleDesires.Add(DesireLeave);
                                if (gs.isVeryLate()) possibleDesires.Add(DesireLeave);
                                if (gs.isVeryLate()) possibleDesires.Add(DesireLeave);
                                if (gs.isVeryLate()) possibleDesires.Add(DesireLeave);
                                if (gs.isVeryLate()) possibleDesires.Add(DesireLeave);
                            }
                            break;
                        default:
                            possibleDesires = {DesireNothing}
                    };
                    int desireIndex = Random.Next(0, possibleDesires.length)
                    Desire = possibleDesires[desireIndex];
                };
                //
                // change action based on desire
                //
                switch (Desire)
                {
                    case DesireTalk:
                        if (distanceToNearestGuest <= AdjacentDistance) {
                            Action = Conversation;
                            Desire = DesireNothing;
                        } else {
                            Action = Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    case DesireDance:
                        if (distanceToNearestBar <= NearDistance) {
                            Action = Dance;
                            Desire = DesireNothing;
                        } else {
                            Action = Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    case DesireDrink:
                        if (distanceToNearestBar <= AdjacentDistance) {
                            Action = Drinking;
                            Desire = DesireNothing;
                            // TODO: handle drink purchase
                        } else {
                            Action = Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    case DesireLeave:
                        if (distanceToFrontDoor <= AdjacentDistance) {
                            // TODO: handle "exit" of guest
                            Action = Idle;
                            // we do NOT reset desire once the desire to leave starts
                        }
                            Action = Seeking;
                            // TODO: add logic for choosing target
                        };
                        break;
                    default:
                        Action = Idle;
                        Desire = DesireNothing;
                }
                //
                // change animation based on Action
                //
                switch (Action) {
                    case Conversation:
                        CurrentAnimation = AnimIdle;
                        MovementSpeed = 0;
                        break;
                    case Wander:
                    case Seeking:
                        if (InebriationLevel >= 3) {
                            CurrentAnimation = AnimMovingDrunk;
                            MovementSpeed = 2;
                        } else {
                            CurrentAnimation = AnimMovingSober;
                            MovementSpeed = 3;
                        }
                        break;
                    case Drinking:
                        CurrentAnimation = AnimDrinking;
                        MovementSpeed = 0;
                        break;
                    case MixingDrink: // only done by bartender (if we make one)
                        CurrentAnimation = AnimMixing;
                        MovementSpeed = 0;
                        break;
                    case Serving: // only done by bartender (if we make one)
                        CurrentAnimation = AnimServing;
                        MovementSpeed = 0;
                        break;
                    case Dancing:
                        CurrentAnimation = AnimDancing;
                        MovementSpeed = 0;
                        break;
                    case Leaving:
                        if (gs.isMorning()) {
                            CurrentAnimation = AnimRunning;
                            MovementSpeed = 7;
                        } else {
                            if (InebriationLevel >= 3) {
                                CurrentAnimation = AnimMovingDrunk;
                                MovementSpeed = 2;
                            } else {
                                CurrentAnimation = AnimMovingSober;
                                MovementSpeed = 3;
                            }
                        }
                        break;
                    case Screaming:
                        CurrentAnimation = AnimAnger;
                        MovementSpeed = 0;
                        break;
                    case Flirting: // Only done by player
                    case Cleaning: // Only done by player
                        CurrentAnimation = AnimIdle; 
                        MovementSpeed = 0;
                        break;
                    default:
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
