using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveProject.DecisionManager
{
    // Secuencia de acciones, tal y como está en el libro
    public class ActionSequence : GenericAction
    {
        public GenericAction[] Actions { get; set; }

        public int CurrentAction { get; set; }

        public ActionSequence(GenericAction[] actions)
            : base(0, 0, false, null)
        {
            Actions = actions;
        }

        public override bool CanInterrupt()
        {
            return Actions[0].CanInterrupt();
        }

        public override bool CanDoBoth(GenericAction otherAction)
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
