using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sarachan.UniTiya.TiyaPropertyAttributes;

namespace Sarachan.UniTiya
{
    /// <summary>
    /// UniTiya 游戏内容的全局对象获取入口。
    /// 在一个 Scene 中应该有且仅有一个该组件。
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Tiya Game System", -999)]
    [DisallowMultipleComponent]
    public sealed class TiyaGameSystem : MonoBehaviour
    {
        //[SerializeField] UnityEngine.InputSystem.InputActionAsset _inputActions;

        [SerializeField] [TypeRestriction(typeof(IGameObjectPool))] Object _poolObject;
        static IGameObjectPool _pool;

        [SerializeField] [TypeRestriction(typeof(IActorController))] Object _playerObject;
        static IActorController _playerActor;

        [SerializeField] [TypeRestriction(typeof(IItemManager))] Object _itemManagerObject;
        static IItemManager _itemManager;
        
        // Singleton
        static TiyaGameSystem _instance = null;

        /// <summary>
        /// 单例对象
        /// </summary>
        public static TiyaGameSystem Instance => _instance ? 
            _instance : throw new MissingComponentException($"There must be at least one instance of ${nameof(TiyaGameSystem)} component in this scene.");

        /// <summary>
        /// 对象池
        /// </summary>
        public static IGameObjectPool Pool => _pool;
        /// <summary>
        /// 玩家 ActorController
        /// </summary>
        public static IActorController PlayerActor => _playerActor;
        /// <summary>
        /// 玩家 View
        /// </summary>
        public static IActorCameraView PlayerView => PlayerActor.ActorView as IActorCameraView;
        /// <summary>
        /// 系统 ItemManager
        /// </summary>
        public static IItemManager ItemManager => _itemManager;

        private void Awake()
        {
            if (!_instance)
            {
                _instance = this;
            }

            _playerActor = _playerObject.ConvertTo<IActorController>();
            _pool = _poolObject.ConvertTo<IGameObjectPool>();
            _itemManager = _itemManagerObject.ConvertTo<IItemManager>();
        }

        private void Start()
        {
            foreach (var trans in PlayerActor.GameObject.GetComponentsInChildren<Transform>())
            {
                trans.gameObject.layer = LayerMask.NameToLayer("Player");
            }
        }
    }
}
