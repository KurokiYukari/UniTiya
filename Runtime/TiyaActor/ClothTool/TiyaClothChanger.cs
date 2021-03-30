using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Sarachan.UniTiya.TiyaActor.ClothTool
{
    /// <summary>
    /// 暖暖换衣服组件
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Actor/Cloth Tools/Tiya Cloth Changer")]
    public class TiyaClothChanger : MonoBehaviour
    {
        #region Unity Inspector
        /// <summary>
        /// 默认 Cloth
        /// </summary>
        public GameObject DefaultClothPrefab;

        public TiyaActorModelInfo CInfo;

#if DYNAMIC_BONE
        [Header("如果启用，则不使用该脚本的 DynamicBoneColliders")]
        public bool UseAllDBColliders = true;

        /// <summary>
        /// 要计算的 Body 的 DynamicBoneColliders，只在 <see cref="UseAllDBColliders"/> 为 False 时生效
        /// （如果一个 GameObject 上有多个 DynamicCollider 组件，只用添加一次即可，该脚本会在 Awake 自动计算其他）
        /// </summary>
        public DynamicBoneColliderBase[] DynamicBoneColliders;
#endif

        [Space(10)]
        [Header("Events")]
        public ClothChangeEvent OnClothChangeStart;

        // TODO: 此时 EventData.FromCloth 已经被 Destory 是否有影响？
        public ClothChangeEvent OnClothChangeFinish;

        public ClothCmptHideEvent OnHideClothCmptStart;

        public ClothCmptHideEvent OnHideClothCmptFinish;
        #endregion

        private TiyaClothInfo _currentCloth;

        #region Unity Events
        private void Awake()
        {
#if DYNAMIC_BONE
            if (UseAllDBColliders)
            {
                DynamicBoneColliders = CInfo.DynamicBoneColliders;
            }
#endif
        }

        private void Start()
        {
            ChangeClothTo(DefaultClothPrefab);
        }
        #endregion

        /// <summary>
        /// 按 Cloth Optional Mesh 在数组中的位置 index 隐藏或显示其自身
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hide"></param>
        public void HideClothComponent(int index, bool hide = true)
        {
            SkinnedMeshRenderer clothCmpt;
            try
            {
                clothCmpt = _currentCloth.OptionalMeshRenderers[index];
            }
            catch (System.IndexOutOfRangeException)
            {
                Debug.LogError($"Index {index} of {nameof(_currentCloth.OptionalMeshRenderers)} (Length: {_currentCloth.OptionalMeshRenderers.Length}) out of range");
                return;
            }

            // Event Data
            var eData = new ClothCmptHideEvent.EventData(clothCmpt, hide);
            // Invoke OnHideClothCmptStart
            OnHideClothCmptStart.Invoke(this, eData);

            clothCmpt.enabled = !hide;

            // Invoke OnHideClothCmptFinish
            OnHideClothCmptFinish.Invoke(this, eData);
        }

        /// <summary>
        /// 将 clothPrefab 附加到当前 Model 上
        /// </summary>
        /// <param name="clothPrefab">一个具有 ClothInfo 组件的 GameObject</param>
        public void ChangeClothTo(GameObject clothPrefab)
        {
            // 创建 cloth
            GameObject clothObj = Instantiate(clothPrefab, transform);
            var targetClothInfo = clothObj.GetComponent<TiyaClothInfo>();

            Debug.Assert(targetClothInfo, $"{clothPrefab.name} doesnt have {nameof(TiyaClothInfo)} Component!");

            // Event Data
            var eData = new ClothChangeEvent.EventData(_currentCloth, targetClothInfo);
            // Invoke OnChangeClothStart
            OnClothChangeStart.Invoke(this, eData);

            // 清空当前 cloth
            ClearCloth();

            // 将新的骨骼添加到 Body 骨架中
            List<Transform> bodyBones = new List<Transform>(CInfo.BodySmrs[0].bones);
            foreach (var bone in targetClothInfo.AddedBonesRoots)
            {
                var parentName = bone.parent.name;
                bone.SetParent(bodyBones.Find(x => x.name.Equals(parentName)), false);
            }

            // 更新 Body 的 Mesh 中的骨骼信息
            // 计算结果
            foreach (var bone in targetClothInfo.AddedBonesRoots)
            {
                var addedBones = bone.GetComponentsInChildren<Transform>();
                for (int i = 1; i < addedBones.Length; i++) // 从 1 开始，因为根结点不是骨骼结点
                {
                    bodyBones.Add(addedBones[i]);
                }
            }
            // 更新所有 Mesh
            var bodyBonesArray = bodyBones.ToArray();
            foreach (var smr in CInfo.BodySmrs)
            {
                smr.bones = bodyBonesArray;
            }

            // 将 Cloth 的 Mesh 的 bones 绑定到 Body 的骨骼
            var clothBones = targetClothInfo.AllMeshRenderers[0].bones;
            // 更新第一个 Mesh
            for (int i = 0; i < clothBones.Length; i++)
            {
                clothBones[i] = bodyBones.Find(x => x.name.Equals(clothBones[i].name));
            }
            // 更新所有 Mesh
            foreach (var smr in targetClothInfo.AllMeshRenderers)
            {
                smr.bones = clothBones;
            }

#if DYNAMIC_BONE
            // 处理碰撞
            // 在 Cloth 的所有 DynamicBones 中添加自身的所有 DynamicCollider
            foreach (var dynamicBone in targetClothInfo.DynamicBones)
            {
                dynamicBone.m_Colliders.AddRange(DynamicBoneColliders);
            }
#endif

            // 换装结束
            _currentCloth = targetClothInfo;

            // Invoke OnChangeClothFinish
            OnClothChangeFinish.Invoke(this, eData);
        }

        /// <summary>
        /// 清空 Cloth，将模型还原到初始状态
        /// </summary>
        private void ClearCloth()
        {
            if (_currentCloth == null)
                return;

            // 将自身 Mesh 中的骨骼信息还原
            var bonesArray = new Transform[CInfo.BodyBonesCount];
            System.Array.Copy(CInfo.BodySmrs[0].bones, bonesArray, CInfo.BodyBonesCount);
            foreach (var smr in CInfo.BodySmrs)
            {
                smr.bones = bonesArray;
            }

            // 销毁对象
            // 将新骨骼实体删除
            foreach (Transform bone in _currentCloth.AddedBonesRoots)
            {
                Destroy(bone.gameObject);
            }

            // 销毁 Cloth GameObject
            Destroy(_currentCloth.gameObject);

            _currentCloth = null;
        }

        /// <summary>
        /// 更换 cloth 的 Event
        /// </summary>
        [System.Serializable]
        public class ClothChangeEvent : UnityEvent<TiyaClothChanger, ClothChangeEvent.EventData>
        {
            public readonly struct EventData
            {
                /// <summary>
                /// 为更换时的 Invoker._currentCloth
                /// </summary>
                public TiyaClothInfo FromCloth { get; }

                /// <summary>
                /// 目标 cloth
                /// </summary>
                public TiyaClothInfo ToCloth { get; }

                public EventData(TiyaClothInfo fromCloth, TiyaClothInfo toCloth)
                {
                    FromCloth = fromCloth;
                    ToCloth = toCloth;
                }
            }
        }

        /// <summary>
        /// 第一个参数是暖暖自身；第二个参数是 cloth；第三个参数是 clothCmpt
        /// </summary>
        [System.Serializable]
        public class ClothCmptHideEvent : UnityEvent<TiyaClothChanger, ClothCmptHideEvent.EventData>
        {
            public readonly struct EventData
            {
                /// <summary>
                /// 目标 clothCmpt
                /// </summary>
                public SkinnedMeshRenderer ClothCmpt { get; }

                /// <summary>
                /// 操作种类。True 为 Hide 操作，False 为 Show。
                /// </summary>
                public bool ActionHide { get; }

                public EventData(SkinnedMeshRenderer clothCmpt, bool actionHide)
                {
                    ClothCmpt = clothCmpt;
                    ActionHide = actionHide;
                }
            }
        }
    }
}
