using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaActor.AI
{
    [RequireComponent(typeof(TiyaActorMoveAI))]
    public class TiyaPatrolAI : MonoBehaviour
    {
        [SerializeField] bool _stopNavigationWhenDisable = false;

        [SerializeField] List<Transform> _patrolTransforms;
        public List<Transform> PatrolTransforms => _patrolTransforms;

        int _currentDestinationIndex = -1;

        TiyaActorMoveAI _actorMoveAI;
        public TiyaActorMoveAI ActorMoveAI => _actorMoveAI ??= GetComponent<TiyaActorMoveAI>();

        protected void Start()
        {
            ActorMoveAI.Agent.autoBraking = false;
        }

        protected void OnEnable()
        {
            ActorMoveAI.enabled = true;

            ActorMoveAI.OnArriveDestination += SwitchToNextDestination;

            SwitchToNextDestination();
        }

        protected void OnDisable()
        {
            if (_stopNavigationWhenDisable)
            {
                ActorMoveAI.enabled = false;
            }

            ActorMoveAI.OnArriveDestination -= SwitchToNextDestination;
        }

        void SwitchToNextDestination()
        {
            if (PatrolTransforms.Count != 0)
            {
                _currentDestinationIndex = (_currentDestinationIndex + 1) % PatrolTransforms.Count;
                ActorMoveAI.Destination = PatrolTransforms[_currentDestinationIndex].position;
            }
        }
    }
}
