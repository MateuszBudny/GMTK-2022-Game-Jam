using Aether;

namespace AetherEvents
{
    public class ALittleBitOfSanityLost : Event<ALittleBitOfSanityLost>
    {
        public readonly float instanityProgress;

        public ALittleBitOfSanityLost(float instanityProgress)
        {
            this.instanityProgress = instanityProgress;
        }
    }
}