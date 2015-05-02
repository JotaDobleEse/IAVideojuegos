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
            
        }

    }
}
