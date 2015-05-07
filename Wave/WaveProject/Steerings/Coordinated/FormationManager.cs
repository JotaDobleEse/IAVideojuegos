using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings.Coordinated
{
    public class SlotAssignment
    {
        public Kinematic Character { get; set; }
        public int SlotNumber { get; set; }
    };

    public class FormationManager
    {
        public List<SlotAssignment> SlotAssignments { get; set; }
        public SlotLocation DriftOffset { get; set; }
        public Pattern Pattern { get; set; }
        public Matrix3x3 OrientationMatrix { get; set; }

        public void UpdateSlotAssignments()
        {
            for (int i = 0; i < SlotAssignments.Count; i++)
            {
                SlotAssignments[i].SlotNumber = i;
            }
            DriftOffset = Pattern.GetDriftOffset(SlotAssignments);
        }

        public bool AddCharacter(Kinematic character)
        {
            if (Pattern.SupportSlots(SlotAssignments.Count + 1))
            {
                SlotAssignment slot = new SlotAssignment();
                slot.Character = character;
                SlotAssignments.Add(slot);

                UpdateSlotAssignments();
                return true;
            }
            return false;
        }

        public void RemoveCharacter(Kinematic character)
        {
            if (SlotAssignments.Any(a => a.Character == character) && SlotAssignments.Remove(SlotAssignments.First(f => f.Character == character)))
            {
                UpdateSlotAssignments();
            }
        }

        private SlotLocation getAnchorPoint()
        {
            /*** TODA LA INVENTADA PADRE ***/
            //Centro de masas + driftOffset * Velocidad centro de masas
            SlotLocation pAnchor = new SlotLocation();
            Vector2 pos = Vector2.Zero;
            Vector2 vel = Vector2.Zero;
            foreach (var character in SlotAssignments)
            {
                pos += character.Character.Position;
                vel += character.Character.Velocity;
            }
            pos /= SlotAssignments.Count;
            vel /= SlotAssignments.Count;

            pAnchor.Position = pos + DriftOffset.Position * vel;
            pAnchor.Orientation = pAnchor.Position.ToRotation();

            return pAnchor;
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
            OrientationMatrix = new Matrix3x3(cos, -sin, 0, 
                                             sin, cos, 0, 
                                             0, 0, 1);


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
                                                            relativeLoc.Position.X,relativeLoc.Position.Y,1);


                //Deja el resultado en relativeLoc, esperemos
                Matrix3x3.Multiply(OrientationMatrix, relativeLocMatrix);
                
               

                //Obtenemos los nuevos valores de la matriz despues de la multiplicacion
                var newPos = new Vector2(relativeLocMatrix.M31, relativeLocMatrix.M32);
                //Sumamos el offset
                newPos += positionOffset;

                //Asignamos al caracter la nueva posicion y la nueva orientacion
                slot.Character.Position = newPos;
                slot.Character.Orientation = relativeLoc.Orientation + orientationOffset;



                
            }
            /*for (int i = 0; i < slotAssignments.size; i++) {
              SlotAssignment<T> slotAssignment = slotAssignments.get(i);

              // Retrieve the location reference of the formation member to calculate the new value
              Location<T> relativeLoc = slotAssignment.member.getTargetLocation();

              // Ask for the location of the slot relative to the anchor point
              pattern.calculateSlotLocation(relativeLoc, slotAssignment.slotNumber);

              T relativeLocPosition = relativeLoc.getPosition();

             * 
              if (relativeLocPosition instanceof Vector2)
                  ((Vector2)relativeLocPosition).mul(orientationMatrix);
              else if (relativeLocPosition instanceof Vector3) ((Vector3)relativeLocPosition).mul(orientationMatrix);

              // Add the anchor and drift components
              relativeLocPosition.add(positionOffset);
              relativeLoc.setOrientation(relativeLoc.getOrientation() + orientationOffset);
          }*/


        }

    }
}
