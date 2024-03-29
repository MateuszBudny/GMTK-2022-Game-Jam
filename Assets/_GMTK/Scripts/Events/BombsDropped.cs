using Aether;

namespace AetherEvents
{
    public class BombsDropped : Event<BombsDropped>
    {
        public int WhichDroppingIsThis { get; }
        public int DroppingsNumToGoIntoMadness { get; }

        public BombsDropped(int whichDroppingIsThis, int droppingsNumToGoIntoMadness)
        {
            WhichDroppingIsThis = whichDroppingIsThis;
            DroppingsNumToGoIntoMadness = droppingsNumToGoIntoMadness;
        }
    }
}