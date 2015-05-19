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
        // Acciones en cola
        public List<GenericAction> Actions { get; set; }
        // Acciones activas
        public List<GenericAction> Active { get; private set; }
        // Tiempo para siguiente ejecución
        public const float ExecutionTime = 0.5f;
        private float CurrentTime = 0f;

        public ActionManager()
        {
            Actions = new List<GenericAction>();
            Active = new List<GenericAction>();
        }

        // Obtiene la siguiente acción ordenando por prioridad
        private GenericAction NextAction()
        {
            return Actions.OrderBy(o => o.Priority).FirstOrDefault();
        }

        // Obtiene la maxima prioridad entre las acciones activas
        private int GetHighestPriority()
        {
            return Active.Select(s => s.Priority).OrderBy(o => o).FirstOrDefault();
        }

        // Añade una acción a la cola
        public void ScheduleAction(GenericAction action)
        {
            Actions.Add(action);
        }

        public string Execute(float dt)
        {
            string result = "";
            // Restamos tiempo de vida a las acciones en cola
            foreach (var action in Actions)
            {
                action.ExpireTime -= dt;
            }

            // Restamos tiempo de vida a las acciones activas
            foreach (var action in Active)
            {
                action.ExpireTime -= dt;
            }

            // Sumamos tiempo desde la última ejecución
            CurrentTime += dt;
            // Si el tiempo desde la última ejecución es superior o igual al tiempo de ejecución, seguimos
            if (CurrentTime >= ExecutionTime)
            {
                // Quitamos las acciones de la cola cuyo tiempo ha expirado
                Actions = Actions.Where(w => w.ExpireTime > 0).ToList();

                // Obtenemos la siguiente acción
                GenericAction nextAction = NextAction();
                // Si la siguiente acción tiene menos prioridad que la acción de mayor prioridad
                if (nextAction != null && GetHighestPriority() <= nextAction.Priority)
                {
                    // Si se puede interrumpir la siguinete acción puede interrumpir,
                    // la establecemos como la única activa
                    if (nextAction.CanInterrupt())
                    {
                        Active.Clear();
                        Active.Add(nextAction);
                    }
                }

                // Para cada acción de la cola ordenada por prioridad
                foreach (var action in Actions.OrderBy(o => o.Priority))
                {
                    // Comprobamos si se puede ejecutar el resto de acciones activas
                    bool ok = true;
                    foreach (var activeAction in Active)
                    {
                        if (!action.CanDoBoth(activeAction))
                        {
                            ok = false;
                            break;
                        }
                    }
                    // Si se puede ejecutar con todas las activas se añade a la lista de activas
                    if (ok)
                    {
                        Actions.Remove(action);
                        Active.Add(action);
                    }
                }

                // Si no hay acciones activas, elegimos la siguiente
                if (Active.Count == 0)
                {
                    GenericAction action = NextAction();
                    Actions.Remove(action);
                    Active.Add(action);
                }

                // Eliminamos las acciones que hayan acabado
                Active = Active.Where(w => w != null && !w.IsComplete()).ToList();

                if (Active.Count > 0)
                    result = Active[0].Function.Method.ToString().Split(' ')[1];

                // Ejecutamos las acciones
                foreach (var activeAction in Active)
                {
                    activeAction.Execute();
                }
                // Reiniciamos el contador de última ejecucíón
                CurrentTime = 0f;
            }
            return result;
        }
    }
}
