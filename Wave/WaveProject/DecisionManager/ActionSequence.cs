using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveProject.DecisionManager
{
    public class ActionSequence : Action
    {
        public Action[] Actions { get; set; }

        public int CurrentAction { get; set; }

        public override bool CanInterrupt()
        {
            return Actions[0].CanInterrupt();
        }

        public override bool CanDoBoth(Action otherAction)
        {
            return !Actions.Any(a => !a.CanDoBoth(otherAction));
        }

        public override bool IsComplete()
        {
            return CurrentAction >= Actions.Length;
        }

        public override void Execute()
        {
            Actions[CurrentAction].Execute();

            if (Actions[CurrentAction].IsComplete())
                CurrentAction++;
        }
    }
}
