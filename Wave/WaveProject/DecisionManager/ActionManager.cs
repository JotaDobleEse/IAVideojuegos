using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveProject.DecisionManager
{
    public class ActionManager
    {
        public List<Action> Actions { get; set; }
        public List<Action> Active { get; private set; }
        public int CurrentTime { get; private set; }

        private Action NextAction()
        {
            return Actions.OrderBy(o => o.Priority).FirstOrDefault();
        }

        private int GetHighestPriority()
        {
            return Active.Select(s => s.Priority).OrderBy(o => o).First();
        }

        public void ScheduleAction(Action action)
        {
            Actions.Add(action);
        }

        public void Execute()
        {
            CurrentTime++;

            Actions = Actions.Where(w => w.ExpireTime > CurrentTime).ToList();

            Action nextAction = NextAction();
            if (nextAction != null && GetHighestPriority() > nextAction.Priority)
            {
                if (nextAction.CanInterrupt())
                {
                    Active.Clear();
                    Active.Add(nextAction);
                }
            }

            foreach (var action in Actions)
            {
                foreach (var activeAction in Active)
                {
                    if (!action.CanDoBoth(activeAction))
                        continue;
                    Actions.Remove(action);
                    Active.Add(action);
                }
            }

            Active = Actions.Where(w => !w.IsComplete()).ToList();

            foreach (var activeAction in Active)
            {
                activeAction.Execute();
            }

        }
    }
}
