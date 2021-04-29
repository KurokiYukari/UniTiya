using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaActor.ClothTool
{
    /// <summary>
    /// Actor Model Prefab 的组件，用于向外界提供 Model 细节信息
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Actor/Cloth Tools/Tiya Actor Model Info")]
    public class TiyaActorModelInfo : MonoBehaviour
    {
        #region Unity Inspector
        /// <summary>
        /// Body 所有 Meshes
        /// </summary>
        public SkinnedMeshRenderer[] BodySmrs;

#if DYNAMIC_BONE
        /// <summary>
        /// 所有 DynamicBoneColliders
        /// （如果一个 GameObject 上有多个 DynamicCollider 组件，只用添加一次即可，该脚本会在 Awake 自动计算其他）
        /// </summary>
        public DynamicBoneColliderBase[] DynamicBoneColliders;
#endif
        #endregion

        /// <summary>
        /// 暖暖初始状态时的骨骼数目
        /// </summary>
        public int BodyBonesCount { get; private set; }

        #region Unity Events
        private void Awake()
        {
#if DYNAMIC_BONE
            List<DynamicBoneColliderBase> allCols = new List<DynamicBoneColliderBase>();
            foreach (var item in DynamicBoneColliders)
            {
                allCols.AddRange(item.GetComponents<DynamicBoneColliderBase>());
            }
            DynamicBoneColliders = allCols.ToArray();
#endif


            if (BodySmrs.Length > 0)
            {
                BodyBonesCount = BodySmrs[0].bones.Length;
            }
        }
        #endregion
    }
}
