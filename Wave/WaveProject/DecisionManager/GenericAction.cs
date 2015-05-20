using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveProject.DecisionManager
{
    // Acción
    public class GenericAction
    {
        // Tiempo para expirar
        public float ExpireTime { get; set; }
        // Prioridad de ejecución
        public int Priority { get; set; }
        // Función a ejecutar
        public Action Function { get; set; }

        // Indica si la acción puede interrumpir
        private bool canInterrupt = true;
        // Indica si la acción ha sido completada
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
            // Si las dos acciones son del tipo "ir a" no se pueden ejecutar juntas
            if (Function.Method.ToString().Contains("GoTo") && otherAction.Function.Method.ToString().Contains("GoTo"))
                return false;
            // Si son iguales no se pueden ejecutar juntas
            return !Function.Method.Equals(otherAction.Function.Method);
        }

        public virtual bool IsComplete()
        {
            return isComplete || (ExpireTime <= 0);
        }

        public virtual void Execute()
        {
            Function.Invoke();
        }
    }
}
