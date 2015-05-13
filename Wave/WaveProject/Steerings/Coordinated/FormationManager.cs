using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveProject.CharacterTypes;
using WaveProject.Steerings.Delegated;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject.Steerings.Coordinated
{
    public class SlotAssignment
    {
        public ICharacterInfo Character { get; set; }
        public int SlotNumber { get; set; }
    };


    public class FormationManager
    {
        public List<SlotAssignment> SlotAssignments { get; set; }
        public List<Kinematic> Positions { get; set; }
        public SlotLocation DriftOffset { get; set; }
        public Pattern Pattern { get; set; }
        public Matrix3x3 OrientationMatrix { get; set; }
        public Kinematic AnchorPoint { get; set; }
        public FollowPath Steering { get; set; }
        public FormationType Type { get; set; }


        public FormationManager()
        {
            SlotAssignments = new List<SlotAssignment>();
            Positions = new List<Kinematic>();
            AnchorPoint = new Kinematic();
            Steering = new FollowPath(true) { Character = AnchorPoint };
            Type = new FormationType();
        }

        public void UpdateSlotAssignments()
        {
            Vector2 position = Vector2.Zero;
            for (int i = 0; i < SlotAssignments.Count; i++)
            {
                SlotAssignments[i].SlotNumber = i;
                position += SlotAssignments[i].Character.GetPostion();
            }
            AnchorPoint.Position = position / SlotAssignments.Count;
            DriftOffset = Pattern.GetDriftOffset(SlotAssignments);
        }

        public bool AddCharacter(ICharacterInfo character)
        {
            if (SlotAssignments.Any(a => a.Character == character))
                return false;
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

        public void RemoveCharacter(ICharacterInfo character)
        {
            if (SlotAssignments.Any(a => a.Character == character) && SlotAssignments.Remove(SlotAssignments.First(f => f.Character == character)))
            {
                UpdateSlotAssignments();
            }
        }

        private SlotLocation getAnchorPoint()
        {
            //Centro de masas + driftOffset * Velocidad centro de masas
            return new SlotLocation() { Position = AnchorPoint.Position/* + DriftOffset.Position * AnchorPoint.Velocity*/, Orientation = AnchorPoint.Orientation };
        }

        public void UpdateSlot()
        {
            var anchor = getAnchorPoint();

            var positionOffset = anchor.Position;
            float orientationOffset = anchor.Orientation;

            //en raidens
            float cos = (float)Math.Cos(orientationOffset);
            float sin = (float)Math.Sin(orientationOffset);

            //el orden es del constructor11,12,13,21,22,23,31,32,33, 
            //supongo que cojera primero la columna 1, luego la columna dos y luego la columna 3

            // cos  sin  0
            // -sin cos  0
            // 0    0    1
            OrientationMatrix = new Matrix3x3(sin, -cos, 0, 
                                             cos, sin, 0, 
                                             0, 0, 1);
            Positions.Clear();
            foreach (var slot in SlotAssignments)
            {
                //Obtenemos el location
                var relativeLoc = Pattern.GetSlotLocation(slot.SlotNumber);

                //Voy a poner la posicion como matriz de desplazamiento 3x3, para que se puedan multiplicar las matrices, columna1, columna2, columna3
                //1 0 X
                //0 1 Y
                //0 0 1
                Matrix3x3 relativeLocMatrix = new Matrix3x3(1,0,0,
                                                            0,1,0,
                                                            relativeLoc.Position.X, relativeLoc.Position.Y, 1);

                //Deja el resultado en relativeLoc, esperemos
                Matrix3x3 result = Matrix3x3.Multiply(OrientationMatrix, relativeLocMatrix);

                //Obtenemos los nuevos valores de la matriz despues de la multiplicacion
                var newPos = result.Translation;
                //Sumamos el offset
                newPos += positionOffset;

                var location = new Kinematic();

                //Asignamos al caracter la nueva posicion y la nueva orientacion
                location.Position = newPos;
                location.Orientation = relativeLoc.Orientation + orientationOffset;

                //Añadimos el componente drift
                location.Position -= DriftOffset.Position;
                location.Orientation -= DriftOffset.Orientation;

                Positions.Add(location);

                slot.Character.SetTarget(location);
            }
        }

        public void MoveToPosition(Vector2 dst)
        {
            LRTA lrta = new LRTA(AnchorPoint.Position, dst, Type, DistanceAlgorith.CHEVYCHEV);
            List<Vector2> path = lrta.Execute();
            Steering.SetPath(path);
        }

        public void Update(float dt)
        {
            UpdateSlot();

            Terrain terrain = Map.CurrentMap.TerrainOnWorldPosition(AnchorPoint.Position);
            AnchorPoint.MaxVelocity = Type.MaxVelocity(terrain);

            SteeringOutput output = Steering.GetSteering();
            AnchorPoint.Update(dt, output);
        }

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
