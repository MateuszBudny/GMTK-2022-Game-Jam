using Aether;

namespace AetherEvents
{
    public class BombsDropped : Event<BombsDropped>
    {
        public int WhichDroppingIsThis { get; }

        public BombsDropped(int whichDroppingIsThis)
        {
            WhichDroppingIsThis = whichDroppingIsThis;
        }
    }
}