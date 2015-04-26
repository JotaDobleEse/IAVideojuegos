using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

namespace WaveProject.Steering
{

    public class CollisionAvoidanceRT :  Steering
    {
        public float MaxAcceleration { get; set; }
        public float Radius { get; set; }
        public EntityManager EntityManager { get; set; }

        public CollisionAvoidanceRT(EntityManager entityManager)
        {
            EntityManager = entityManager;
            MaxAcceleration = 5f;
            Radius = 20f;
        }

        public override void SteeringCalculation(Transform2D origin, Transform2D target, Vector2? characterSpeed = null)
        {
            Linear = Vector2.Zero;
            Angular = 0;
        }

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target = null)
        {
            Radius = (float)Math.Max(origin.Texture.Texture.Width, origin.Texture.Texture.Height) / 2;

            float shortestTime = float.MaxValue;

            Entity firstTarget = null;
            float firstMinSeparation = 0f;
            float firstDistance = 0f;
            Vector2 firstRelativePos = Vector2.Zero;
            Vector2 firstRelativeVel = Vector2.Zero;
            var targets = GetCollisionCandidates(origin);
            foreach (var targ in targets)
            {
                var targetSteering = targ.FindComponent<SteeringBehavior>();
                Vector2 relativePos = targetSteering.Transform.Position - origin.Transform.Position;
                Vector2 relativeVel = targetSteering.Speed - origin.Speed;
                float relativeSpeed = relativeVel.Length();
                float timeToCollision = Vector2.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                float distance = relativePos.Length();
                float minSeparation = distance - relativeSpeed * timeToCollision;
                if (minSeparation < 2 * Radius)
                    continue;

                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = targ;
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
                    relativeP = firstTarget.FindComponent<Transform2D>().Position - origin.Transform.Position;
                }
                else
                {
                    relativeP = firstRelativePos + firstRelativeVel * shortestTime;
                }
                relativeP.Normalize();
                Linear = relativeP * MaxAcceleration;
                origin.Transform.Rotation = (float)Math.Atan2(origin.Speed.X, -origin.Speed.Y);
            }
            else
            {
                if (Linear == Vector2.Zero)
                    Linear = new Vector2(0, -50);
                origin.Transform.Rotation = (float)Math.Atan2(origin.Speed.X, -origin.Speed.Y);
            }
        }

        private IEnumerable<Entity> GetCollisionCandidates(SteeringBehavior origin)
        {
            return EntityManager.AllEntities.Where(w => w.FindComponent<SteeringBehavior>() != null && w.FindComponent<SteeringBehavior>() != origin);
            //if (origin.Speed.X > 0)
            //{
            //    if (origin.Speed.Y > 0)
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X > origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y > origin.Transform.Position.Y);
            //    }
            //    else
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X > origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y < origin.Transform.Position.Y);
            //    }
            //}
            //else
            //{
            //    if (origin.Speed.Y > 0)
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X < origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y > origin.Transform.Position.Y);
            //    }
            //    else
            //    {
            //        return EntityManager.AllEntities.Where(w => w.FindComponent<Transform2D>() != null && w.FindComponent<Transform2D>().Position.X < origin.Transform.Position.X && w.FindComponent<Transform2D>().Position.Y < origin.Transform.Position.Y);
            //    }
            //}
        }
    }
}
