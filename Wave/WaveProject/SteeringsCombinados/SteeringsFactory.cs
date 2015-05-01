using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveProject.Steerings;

namespace WaveProject.SteeringsCombinados
{
    public struct BehaviorAndWeight
    {
        public Steering Behavior { get; set; }
        public float Weight { get; set; }

    };
    public static class SteeringsFactory
    {
        public static BehaviorAndWeight[] Flocking(Kinematic character)
        {
            BehaviorAndWeight[] behaviors = new BehaviorAndWeight[4];

            behaviors[0] = new BehaviorAndWeight() { Behavior = new Separation() { Character = character }, Weight = 0.7f };
            behaviors[1] = new BehaviorAndWeight() { Behavior = new Cohesion() { Character = character }, Weight = 0.4f };
            behaviors[2] = new BehaviorAndWeight() { Behavior = new Alignment() { Character = character }, Weight = 0.4f };
            behaviors[3] = new BehaviorAndWeight() { Behavior = new Wander() { Character = character }, Weight = 0.8f };

            return behaviors;
        }
        public static BehaviorAndWeight[] LeaderFollowing(Kinematic character, Kinematic leader)
        {
            BehaviorAndWeight[] behaviors = new BehaviorAndWeight[3];



            return behaviors;
        }
    }
}
