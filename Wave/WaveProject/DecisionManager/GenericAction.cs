using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveProject.DecisionManager
{
    public class GenericAction
    {
        public float ExpireTime { get; set; }
        public int Priority { get; set; }
        public Action Function { get; set; }

        private bool canInterrupt = true;
        private bool isComplete = false;

        public GenericAction(float expireTime, int priority, bool canInterrupt, Action action)
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

        public virtual bool CanDoBoth(GenericAction otherAction)
        {
            return !Function.Method.Equals(otherAction.Function.Method);
            //return true;
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
