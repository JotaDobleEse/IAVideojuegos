using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveProject.Characters;
using WaveProject.CharacterTypes;
using WaveProject.Steerings.Delegated;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.Steerings.Coordinated
{
    // Asignación de un Slot, contiene le personaje y su número
    public class SlotAssignment
    {
        public ICharacterInfo Character { get; set; }
        public int SlotNumber { get; set; }
    };


    public class FormationManager
    {
        // Slots asignados
        public List<SlotAssignment> SlotAssignments { get; set; }
        // Posiciones de la formación, para debug
        public List<Kinematic> Positions { get; set; }
        // Desplazamiento actual
        public SlotLocation DriftOffset { get; set; }
        // Patrón de la formación
        public Pattern Pattern { get; set; }
        // Matriz de orientación
        public Matrix3x3 OrientationMatrix { get; set; }
        // Punto de anclaje de la formación
        public Kinematic AnchorPoint { get; set; }
        // Steering para seguimiento de caminos
        public FollowPath Steering { get; set; }
        // Atributo de tipo de personaje adaptado a la formación
        public FormationType Type { get; set; }


        public FormationManager()
        {
            SlotAssignments = new List<SlotAssignment>();
            Positions = new List<Kinematic>();
            AnchorPoint = new Kinematic();
            Steering = new FollowPath(true) { Character = AnchorPoint };
            Type = new FormationType();
        }

        // Actualiza las posiciones de los Slots asignados y recalcula
        // el punto de anclaje y el desplazamiento.
        public void UpdateSlotAssignments()
        {
            Vector2 position = Vector2.Zero;
            for (int i = 0; i < SlotAssignments.Count; i++)
            {
                SlotAssignments[i].SlotNumber = i;
                position += SlotAssignments[i].Character.GetPosition();
            }
            AnchorPoint.Position = position / SlotAssignments.Count;
            DriftOffset = Pattern.GetDriftOffset(SlotAssignments);
        }

        // Añade un personaje a la formación, si puede, y recalcula todo
        public bool AddCharacter(ICharacterInfo character)
        {
            // Si el personaje ya pertenece a la formación no lo inserta
            if (SlotAssignments.Any(a => a.Character == character))
                return false;
            // Si se pueden añadir mas personajes
            if (Pattern.SupportSlots(SlotAssignments.Count + 1))
            {
                SlotAssignment slot = new SlotAssignment();
                slot.Character = character;
                SlotAssignments.Add(slot);

                Pattern.CalculateNumberOfSlots(SlotAssignments);
                UpdateSlotAssignments();
                return true;
            }
            return false;
        }

        // Elimina un personaje y actualiza parámetros
        public void RemoveCharacter(ICharacterInfo character)
        {
            // Si el personaje está en la formación y se ha eliminado, recalculamos
            if (SlotAssignments.Any(a => a.Character == character) && SlotAssignments.Remove(SlotAssignments.First(f => f.Character == character)))
            {
                Pattern.CalculateNumberOfSlots(SlotAssignments);
                UpdateSlotAssignments();
            }
        }

        // Devuelve el punto de anclaje
        private SlotLocation GetAnchorPoint()
        {
            return new SlotLocation() { Position = AnchorPoint.Position, Orientation = AnchorPoint.Orientation };
        }

        // Actualiza la posición de los Slots
        public void UpdateSlot()
        {
            var anchor = GetAnchorPoint();

            var positionOffset = anchor.Position;
            float orientationOffset = anchor.Orientation;

            //Coseno y seno de la orientación del pnto de anclaje en radianes
            float cos = (float)Math.Cos(orientationOffset);
            float sin = (float)Math.Sin(orientationOffset);

            // Matriz de orientación
            OrientationMatrix = new Matrix3x3(sin, -cos, 0, 
                                              cos,  sin, 0, 
                                                0,    0, 1);

            Positions.Clear(); // Limpiamos la lista de posiciones
            // Para cada slot asignado
            foreach (var slot in SlotAssignments)
            {
                // Obtenemos el location
                var relativeLoc = Pattern.GetSlotLocation(slot.SlotNumber);

                // Ponemos la posicion como matriz de desplazamiento 3x3, 
                // para que se puedan multiplicar las matrices, columna1, columna2, columna3
                Matrix3x3 relativeLocMatrix = new Matrix3x3(1,0,0,
                                                            0,1,0,
                                                            relativeLoc.Position.X, relativeLoc.Position.Y, 1);

                // Guardamos el resultado de la multiplicación de las matrices de orientación
                // y la matriz de posición relativa.
                Matrix3x3 result = Matrix3x3.Multiply(OrientationMatrix, relativeLocMatrix);

                // Obtenemos los nuevos valores de la matriz despues de la multiplicación
                var newPos = result.Translation;
                // Sumamos el offset
                newPos += positionOffset;

                var location = new Kinematic();

                // Asignamos al personaje la nueva posicion y la nueva orientacion
                location.Position = newPos;
                location.Orientation = relativeLoc.Orientation + orientationOffset;

                // Añadimos el componente drift
                location.Position -= DriftOffset.Position;
                location.Orientation -= DriftOffset.Orientation;

                // Añadimos la posición actualizada
                Positions.Add(location);

                // Le decimos al personaje que vaya a su posición
                slot.Character.SetTarget(location);
            }
        }

        // Mueve la formación por el mapa en base al punto de anclaje
        public void MoveToPosition(Vector2 dst)
        {
            LRTA lrta = new LRTA(AnchorPoint.Position, dst, Type, DistanceAlgorith.CHEVYCHEV);
            List<Vector2> path = lrta.Execute();
            Steering.SetPath(path);
        }

        // Actualiza las posiciones de la formación
        public void Update(float dt)
        {
            UpdateSlot();

            Terrain terrain = Map.CurrentMap.TerrainOnWorldPosition(AnchorPoint.Position);
            AnchorPoint.MaxVelocity = Type.MaxVelocity(terrain);

            SteeringOutput output = Steering.GetSteering();
            AnchorPoint.Update(dt, output);
        }

        // Dibuja el debug
        public void Draw(LineBatch2D lb)
        {
            lb.DrawCircleVM(AnchorPoint.Position, 5, Color.Cyan, 1f);

            foreach (var loc in Positions)
            {
                lb.DrawCircleVM(loc.Position, 5, Color.Blue, 1f);
            }
        }

    }
}
