using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveProject.DecisionManager
{
    public class Action
    {
        public int ExpireTime { get; set; }
        public int Priority { get; set; }
        public Func<object> Function { get; set; }

        private bool canInterrupt = true;
        private bool isComplete = false;

        public Action(int expireTime, int priority, bool canInterrupt, Func<object> action)
        {
            ExpireTime = expireTime;
            Priority = priority;
            this.canInterrupt = canInterrupt;
            Function = action;
        }

        public virtual bool CanInterrupt()
        {
            return canInterrupt;
        }

        public virtual bool CanDoBoth(Action otherAction)
        {
            return true;
        }

        public virtual bool IsComplete()
        {
            return isComplete;
        }

        public virtual void Execute()
        {
            Function.Invoke();
            isComplete = true;
        }
    }
}
