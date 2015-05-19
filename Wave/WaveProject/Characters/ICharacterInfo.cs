using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveProject.CharacterTypes;

namespace WaveProject.Characters
{
    // Interfaz para enlazar personajes, tanto controlables, como NPCs
    public interface ICharacterInfo : IDisposable
    {
        // Obtiene la posición del personaje
        Vector2 GetPosition();
        // Obtiene la velocidad del personaje
        Vector2 GetVelocity();
        // Obtiene el equipo del personaje
        int GetTeam();
        // Obtiene el tipo del personaje
        EnumeratedCharacterType GetCharacterType();
        // Establece la posición a la que ir
        void SetTarget(Kinematic target);
        // Asigna un objetivo para el pathfinding
        void SetPathFinding(Vector2 target);
        // Recupera salud al personaje
        void ReceiveHeal(int hp);
        // Resta salud al personaje
        void ReceiveAttack(int atk);
        // Indica si el personaje está muerto
        bool IsDead();
        // Indica si el personaje ha sido eliminado
        bool IsDisposed();
    }
}
