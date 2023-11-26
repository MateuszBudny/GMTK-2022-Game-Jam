using Aether;

namespace AetherEvents
{
    public class HullOpeningForBombsDroppingStarted : Event<HullOpeningForBombsDroppingStarted>
    {
        public int WhichDroppingIsThis { get; }
        public int DroppingsNumToGoIntoMadness { get; }

        public HullOpeningForBombsDroppingStarted(int whichDroppingIsThis, int droppingsNumToGoIntoMadness)
        {
            WhichDroppingIsThis = whichDroppingIsThis;
            DroppingsNumToGoIntoMadness = droppingsNumToGoIntoMadness;
        }
    }
}