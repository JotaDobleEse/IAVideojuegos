using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveProject.DecisionManager
{
    public class ActionCombination : Action
    {
        public Action[] Actions { get; set; }

        public ActionCombination(Action[] actions)
            : base(0, 0, false, null)
        {
            Actions = actions;
        }

        public override bool CanInterrupt()
        {
            return Actions.Any(a => a.CanInterrupt());
        }

        public override bool CanDoBoth(Action otherAction)
        {
            return !Actions.Any(a => !a.CanDoBoth(otherAction));
        }

        public override bool IsComplete()
        {
            return !Actions.Any(a => !a.IsComplete());
        }

        public override void Execute()
        {
            foreach (var action in Actions)
            {
                action.Execute();
            }
        }
    }
}
