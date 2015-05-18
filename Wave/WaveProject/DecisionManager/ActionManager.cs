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
        public List<GenericAction> Actions { get; set; }
        public List<GenericAction> Active { get; private set; }
        public const float ExecutionTime = 0.3f;
        private float CurrentTime = 0f;
        //public int CurrentTime { get; private set; }

        public ActionManager()
        {
            Actions = new List<GenericAction>();
            Active = new List<GenericAction>();
        }

        private GenericAction NextAction()
        {
            return Actions.OrderBy(o => o.Priority).FirstOrDefault();
        }

        private int GetHighestPriority()
        {
            return Active.Select(s => s.Priority).OrderBy(o => o).FirstOrDefault();
        }

        public void ScheduleAction(GenericAction action)
        {
            Actions.Add(action);
        }

        public void Execute(float dt)
        {
            //CurrentTime++;
            foreach (var action in Actions)
            {
                action.ExpireTime -= dt;
            }

            foreach (var action in Active)
            {
                action.ExpireTime -= dt;
            }

            CurrentTime += dt;
            if (CurrentTime >= ExecutionTime)
            {

                Actions = Actions.Where(w => w.ExpireTime > 0).ToList();

                GenericAction nextAction = NextAction();
                if (nextAction != null && GetHighestPriority() > nextAction.Priority)
                {
                    if (nextAction.CanInterrupt())
                    {
                        Active.Clear();
                        Active.Add(nextAction);
                    }
                }

                foreach (var action in Actions.OrderBy(o => o.Priority))
                {
                    bool ok = true;
                    foreach (var activeAction in Active)
                    {
                        if (!action.CanDoBoth(activeAction))
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {
                        Actions.Remove(action);
                        Active.Add(action);
                    }
                }

                if (Active.Count == 0)
                {
                    GenericAction action = NextAction();
                    Actions.Remove(action);
                    Active.Add(action);
                }

                Active = Active.Where(w => w != null && !w.IsComplete()).ToList();

                foreach (var activeAction in Active)
                {
                    //Console.WriteLine(activeAction.Function.Method);
                    activeAction.Execute();
                }
                CurrentTime = 0f;
            }
        }
    }
}
