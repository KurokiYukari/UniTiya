
using Sarachan.UniTiya.BehaviourTree;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    public abstract class ActorActionNodeBase : LeafNode
    {
        IActorController _actor;
        public IActorController Actor => _actor ??= GetComponent<IActorController>();
    }
}
