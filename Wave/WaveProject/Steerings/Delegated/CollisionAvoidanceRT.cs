using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

namespace WaveProject.Steerings.Delegated
{

    public class CollisionAvoidanceRT :  Steering
    {
        public float MaxAcceleration { get; set; }
        public float Radius { get; set; }

        public CollisionAvoidanceRT(bool stable = false) : base(stable)
        {
            MaxAcceleration = 10f;
            Radius = 20f;
        }

        public override SteeringOutput GetSteering()
        {
            float shortestTime = float.MaxValue;

            Kinematic firstTarget = null;
            float firstMinSeparation = 0f;
            float firstDistance = 0f;
            Vector2 firstRelativePos = Vector2.Zero;
            Vector2 firstRelativeVel = Vector2.Zero;
            var targets = GetCollisionCandidates(Character);
            foreach (var target in targets)
            {
                Vector2 relativePos = target.Position - Character.Position;
                Vector2 relativeVel = target.Velocity - Character.Velocity;
                float relativeSpeed = relativeVel.Length();
                float timeToCollision = Vector2.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                float distance = relativePos.Length();
                float minSeparation = distance - relativeSpeed * timeToCollision;
                if (minSeparation < 2 * Radius)
                    continue;

                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = target;
                    firstMinSeparation = minSeparation;
                    firstDistance = distance;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                }
            }

            if (firstTarget != null)
            {
                Vector2 relativeP = Vector2.Zero;
                if (firstMinSeparation <= 0 || firstDistance < 2 * Radius)
                {
                    relativeP = firstTarget.Position - Character.Position;
                }
                else
                {
                    relativeP = firstRelativePos + firstRelativeVel * shortestTime;
                }
                relativeP.Normalize();

                SteeringOutput steering = new SteeringOutput();
                steering.Linear = relativeP * MaxAcceleration;
                steering.Angular = 0;

                Console.WriteLine(steering.Linear);
                //Character.Orientation = Character.Velocity.ToRotation(); // Despues
                return steering;
            }
            return new SteeringOutput();
        }

        private IEnumerable<Kinematic> GetCollisionCandidates(Kinematic origin)
        {
            return Kinematic.Kinematics.Where(w => w != origin/* && (w.Position - origin.Position).Length() <= Radius * 4*/);
            //return EntityManager.AllEntities.Where(w => w.FindComponent<SteeringBehavior>() != null && w.FindComponent<SteeringBehavior>().Kinematic != origin).Select(s => s.FindComponent<SteeringBehavior>().Kinematic);
        }

        public override void Draw(LineBatch2D lb)
        {
            //lb.DrawCircleVM(Character.Position, Radius, Color.Blue, 1f);
        }

    }
}
