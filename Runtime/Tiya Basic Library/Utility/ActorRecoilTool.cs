using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.Commands;

namespace Sarachan.UniTiya.Utility
{
    [System.Serializable]
    public class ActorRecoilTool
    {
        [SerializeField] float _recoilMinMove = 0.06f;
        [SerializeField] float _recoilMaxMove = 0.12f;
        [SerializeField] float _recoilMinRotate = 1.5f;
        [SerializeField] float _recoilMaxRotate = 4.5f;

        public float RecoilMinMove { get => _recoilMinMove; set => _recoilMinMove = value; }
        public float RecoilMaxMove { get => _recoilMaxMove; set => _recoilMaxMove = value; }
        public float RecoilMinRotate { get => _recoilMinRotate; set => _recoilMinRotate = value; }
        public float RecoilMaxRotate { get => _recoilMaxRotate; set => _recoilMaxRotate = value; }

        public void DoRecoilTo(IActorController actor)
        {
            float recoilDisplacement = Random.Range(RecoilMinMove, RecoilMaxMove);
            // FIXME: 武器的后坐力不知道为什么偶尔会导致人物向上移动
            actor.CommandProcessor.AddCommand(ActorCommands.SimpleMove(-recoilDisplacement * actor.ActorTransform.forward));

            float recoilRotation = Random.Range(RecoilMinRotate, RecoilMaxRotate);
            if (actor.IsPlayer) // Player 的情况下需要操纵 Camera
            {
                var viewController = TiyaGameSystem.PlayerView;
                viewController.TempRotate(-recoilRotation);
            }
        }
    }
}
