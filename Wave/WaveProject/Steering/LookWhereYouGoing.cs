﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

namespace WaveProject.Steering
{
    class LookWhereYouGoing : Steering
    {

        public override void SteeringCalculation(SteeringBehavior origin, SteeringBehavior target)
        {

            var direction = (target.Transform.Position + target.Speed) - origin.Transform.Position;

            if (direction.Length() == 0)
            {
                Linear = Vector2.Zero;
                Angular = 0;
                return;
            }

            Align align = new Align();
            Transform2D newTarget = target.Transform.Clone() as Transform2D;
            newTarget.Rotation = (float)Math.Atan2(direction.X, -direction.Y);
            align.SteeringCalculation(newTarget, origin);

            Angular = align.Angular;
            Linear = Vector2.Zero;
        }
    }
}
