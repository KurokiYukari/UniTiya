using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sarachan.UniTiya.TiyaActor.ClothTool
{
    /// <summary>
    /// 暖暖衣服 Prefab 的组件，用于向外界提供自身的各种信息
    /// 衣服绑定到暖暖身上是根据骨骼的名字绑定的
    /// 可以有多出的骨骼，请将多出的骨骼新建一个父节点，将父节点置于 <see cref="AddedBonesRoots"/> 数组中
    /// </summary>
    [AddComponentMenu(TiyaTools.UniTiyaName + "/Actor/Cloth Tools/Tiya Cloth Info")]
    public class TiyaClothInfo : MonoBehaviour
    {
        #region Unity Inspector
        /// <summary>
        /// Cloth 的基础网格（基础网格以外的网格是可选的
        /// </summary>
        public SkinnedMeshRenderer[] BaseMeshRenderer;

        /// <summary>
        /// Cloth 的可选网格
        /// </summary>
        public SkinnedMeshRenderer[] OptionalMeshRenderers;

        /// <summary>
        /// 相对于穿上该 Cloth 的 Prefab，该 Cloth 多出的骨骼的根结点们
        /// 注意：这个根结点是不在 SkinnedMeshRenderer 的 bones 中的，需要用户自己创建
        /// </summary>
        public Transform[] AddedBonesRoots;

#if DYNAMIC_BONE
        /// <summary>
        /// 所有 DynamicBones
        /// （如果一个 GameObject 上有多个 DynamicBone 组件，只用添加一次即可，该脚本会在 Awake 自动计算其他）
        /// </summary>
        public DynamicBone[] DynamicBones;

        /// <summary>
        /// 所有 DynamicColliders
        /// （如果一个 GameObject 上有多个 DynamicCollider 组件，只用添加一次即可，该脚本会在 Awake 自动计算其他）
        /// </summary>
        public DynamicBoneColliderBase[] DynamicBoneColliders;
#endif

        #endregion

        /// <summary>
        /// Cloth 的所有网格（Base + Optional）
        /// </summary>
        public SkinnedMeshRenderer[] AllMeshRenderers { get; private set; }

        #region Unity Events
        private void Awake()
        {
            var allMeshes = new List<SkinnedMeshRenderer>(BaseMeshRenderer);
            allMeshes.AddRange(OptionalMeshRenderers);
            AllMeshRenderers = allMeshes.ToArray();

#if DYNAMIC_BONE
            List<DynamicBone> allBones = new List<DynamicBone>();
            foreach (var item in DynamicBones)
            {
                allBones.AddRange(item.GetComponents<DynamicBone>());
            }
            DynamicBones = allBones.ToArray();

            List<DynamicBoneColliderBase> allCols = new List<DynamicBoneColliderBase>();
            foreach (var item in DynamicBoneColliders)
            {
                allCols.AddRange(item.GetComponents<DynamicBoneColliderBase>());
            }
            DynamicBoneColliders = allCols.ToArray();
#endif
        }
        #endregion
    }
}
