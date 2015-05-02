using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveProject.Steerings.Coordinated
{
    public struct SlotLocation
    {
        public Vector2 Position { get; set; }
        public float Orientation { get; set; }
    };

    public abstract class Pattern
    {
        public int NumberOfSlots { get; set; }

        public abstract SlotLocation GetDriftOffset(List<SlotAssignment> slotAssignments);

        public abstract SlotLocation GetSlotLocation(int slotNumber);

        public virtual bool SupportSlots(int slotCounts)
        {
            return (slotCounts <= NumberOfSlots);
        }
    }
}
