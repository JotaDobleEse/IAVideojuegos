using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Managers;
using WaveProject.Characters;
using WaveProject.DecisionManager;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.CharacterTypes
{
    public class ExplorerCharacter : CharacterType
    {
        public ExplorerCharacter(ICharacterInfo myInfo, EntityManager entityManager)
            : base(myInfo, entityManager, 180, 20, 30, 340) // Párametros iniciales
        {
            // HP    = 180
            // Atk   =  20
            // Def   =  30
            // Radio = 340
        }

        // Coste del explorador al pasar por un terreno
        public override float Cost(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 1.5f;
                case Terrain.FOREST:
                    return 0.5f;
                case Terrain.PATH:
                    return 5f;
                case Terrain.PLAIN:
                    return 2.5f;
                case Terrain.WATER:
                    return 8f;
            }
            return 1;
        }

        // Velocidad máxima del explorador al pasar por un terreno
        public override float MaxVelocity(Terrain terrain)
        {
            switch (terrain)
            {
                case Terrain.DESERT:
                    return 35;
                case Terrain.FOREST:
                    return 40;
                case Terrain.PATH:
                    return 20;
                case Terrain.PLAIN:
                    return 28;
                case Terrain.WATER:
                    return 10;
            }
            return 30;
        }

        public override EnumeratedCharacterType GetCharacterType()
        {
            return EnumeratedCharacterType.EXPLORER;
        }

        // Árbol de decisión
        public override GenericAction Update()
        {
            //ATAQUE
            if (HP >= MaxHP * 0.75)
            {
                var enemy = FindEnemyNear();
                if (enemy != null)
                {
                    return new GenericAction(1f, 1, false, AttackEnemyNear);
                }
                else
                {
                    //SI NO ENCONTRAMOS ENEMIGO NOS DIRIJIMOS A LA BASE ENEMIGA (o a un waypoint, no se)
                    return new GenericAction(1f, 1, false, GoToEnemyBase);
                }
            }
            //DEFENSA
            else
            {
                var enemy = FindEnemyNear();
                //SI ENCONTRAMOS UN ENEMIGO Y
                if (enemy != null)
                {
                    if (Map.CurrentMap.IsInWaypoint(MyInfo.GetPosition()))
                        return new GenericAction(60f, 1, true, GoToHeal);
                    return new GenericAction(1f, 1, false, GoToWaypoint);
                }
                //SI NO ENCONTRAMOS ENEMIGO CERCA 
                return new GenericAction(1f, 1, true, GoToHeal);
            }
        }


        public override void Attack(ICharacterInfo character)
        {
            if (character == null)
                return;
            // Buscamos los tiles alrededor del objetivo
            var attackPoint = Map.CurrentMap.TilePositionByWolrdPosition(character.GetPosition());
            List<Vector2> ps = new List<Vector2>();
            ps.Add(attackPoint + new Vector2(1, 1));
            ps.Add(attackPoint + new Vector2(1, -1));
            ps.Add(attackPoint + new Vector2(-1, 1));
            ps.Add(attackPoint + new Vector2(-1, -1));
            ps.Add(attackPoint + new Vector2(0, 1));
            ps.Add(attackPoint + new Vector2(0, -1));
            ps.Add(attackPoint + new Vector2(1, 0));
            ps.Add(attackPoint + new Vector2(-1, 0));
            
            // Comprobamos si estamos en uno de ellos
            var myPos = Map.CurrentMap.TilePositionByWolrdPosition(MyInfo.GetPosition());
            var isInAttackPos = ps.Any(a => a == myPos);

            // Si lo estamos atacamos
            if (isInAttackPos)
            {
                character.ReceiveAttack(base.Atk);
            }
            // Sino buscamos el punto mas cercano para atacar
            else
            {
                float minLength = float.PositiveInfinity;
                foreach (var pos in ps)
                {
                    if (Map.CurrentMap.Exists(pos) && Map.CurrentMap.NodeMap[pos.X(), pos.Y()].Passable)
                    {
                        var worldPos = Map.CurrentMap.WorldPositionByTilePosition(pos);
                        var length = (worldPos - MyInfo.GetPosition()).Length();
                        if (length < minLength && !EntityManager.PositionOcupped(MyInfo, worldPos))
                        {
                            minLength = length;
                            attackPoint = worldPos;
                        }
                    }
                }

                // Vamos hacia el enemigo
                MyInfo.SetPathFinding(attackPoint);
            }
        }
    }
}
